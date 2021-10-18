using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MBADCases.Authentication;
using MBADCases.Models;
using MBADCases.Services;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace MBADCases.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthWix("wix")]
    public class SchemasController : ControllerBase
    {
        private readonly CaseTypeService _casetypes;
        public SchemasController(CaseTypeService casetypes)
        {
            _casetypes = casetypes;
        }
        [Route("")]
        [Route("controller")]
        [HttpPost]
        public string Index()
        {
            helperservice.LogWixMessages("Index", "");

            //List<WixDB.Schema> lsch = new List<WixDB.Schema>();
            //WixDB.Schema osch = new WixDB.Schema
            //{
            //    DisplayName = "Car",
            //    _Id = "car",
            //    AllowedOperations = new string[] { "get", "find", "count", "update", "insert", "remove" }



            //};
            //lsch.Add(osch);
            //return Newtonsoft.Json.JsonConvert.SerializeObject(lsch);
            return "{}";
        }

        [Route("find")]
        [Route("find/{id?}")]
        [HttpPost]
      
        public IActionResult find(WixDB.find id)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
                _casetypes.Gettenant(tenantid);
                srequest= Newtonsoft.Json.JsonConvert.SerializeObject(id);

                List<WixDB.Schema> lsch = new List<WixDB.Schema>();
                scasetypes = id.schemaIds.ToString();
                foreach (string o in id.schemaIds)
                {
                    List<CaseType> oct = _casetypes.Searchcasetypes(o,false);
                    if (oct != null && oct.Count > 0)
                    {
                        WixDB.Schema osch = GetSchema(oct[0], 50, 3600);
                        lsch.Add(osch);
                    }
                }
                WixDB.DBSchemas osc = new WixDB.DBSchemas { Schemas = lsch };
                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(osc);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, osc);
            }
            catch(Exception ex)
            {
                _casetypes.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix Find", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                throw;
            }
            finally
            {
                _casetypes.SetMessage(new Message() { Callerid="Wix", Callerrequest=srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype="Wix Find", MessageDesc=smessage, Tenantid= tenantid,Userid=usrid });
            }
          
        }

        [Route("list")]
        [Route("list/{id?}")]
        [HttpPost]
        public IActionResult list(WixDB.find id)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string sresponse = "";
            try
            {

                _casetypes.Gettenant(tenantid);

                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(id);

                List<WixDB.Schema> lsch = new List<WixDB.Schema>();

                //get all case types
                List<CaseType> oct = _casetypes.Searchcasetypes("all",false);

                foreach (CaseType o in oct)
                {
                    WixDB.Schema osch = GetSchema(o, 50, 3600);
                    lsch.Add(osch);
                }

                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(lsch);

                WixDB.DBSchemas osc = new WixDB.DBSchemas { Schemas = lsch };

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, osc);
            }
            catch (Exception ex)
            {
                _casetypes.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = "list", Callertype = "Wix list", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                throw;
            }
            finally
            {
                _casetypes.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse= sresponse, Callerrequesttype = "list", Callertype = "Wix list", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
            }

        }
        

        private static List<WixDB.Schema> GetAllSchemasfromDB()
        {
            string spath =   "Models/Schemas.json";
            string sSchemas = System.IO.File.ReadAllText(spath);
            List<WixDB.Schema> lsch =   Newtonsoft.Json.JsonConvert.DeserializeObject<List<WixDB.Schema>>(sSchemas);

            return lsch;
        }

        private static WixDB.Schema GetSchema(CaseType casetype,int maxPage, int ttl)
        {
            WixDB.Schema osch = new WixDB.Schema
            {
                DisplayName = casetype.Casetype ,
                Id = casetype.Casetype,
                AllowedOperations = new string[] { "get", "find", "count", "update", "insert", "remove" },
                MaxPageSize = maxPage,
                ttl = ttl,
                //Fields = GetFields(name)
            };
            IDictionary<string, FieldValue> ofields = GetWixFields();

            foreach (Activity act in casetype.Activities)
            {
                foreach (Models.Action a in act.Actions)
                {
                    IComparer<Models.Casetypefield> comparer = new MyCaseTypeFieldOrder();
                    a.Fields.Sort(comparer);
                    foreach (Casetypefield f in a.Fields)
                    {
                        ofields.Add(new KeyValuePair<string, FieldValue>(f.Fieldid.ToLower(),
                                    new FieldValue()
                                    {
                                        DisplayName = f.Fieldname,
                                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                                        Type = f.Type.ToLower()
                                    })
                                );
                    }
                }
            }
            if (casetype.Fields != null)
            {
                foreach (Casetypefield f in casetype.Fields)
                {
                    ofields.Add(new KeyValuePair<string, FieldValue>(f.Fieldid.ToLower(),
                                        new FieldValue()
                                        {
                                            DisplayName = f.Fieldname,
                                            QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                                            Type = f.Type.ToLower()
                                        })
                                    );
                }
            }
            osch.Fields = ofields;
            
         
            return osch;
        }
        private static IDictionary<string,FieldValue> GetFields(string name)
        {
            IDictionary<string, FieldValue> ofields=null;
            switch (name.ToLower())
            {
                case "tenants":
                    ofields= GetTenantFields();
                    break;
                case "casetypes":
                    ofields = GetCaseTypes();
                    break;
                case "casetypefields":
                    ofields = GetCaseTypeFields();
                    break;
            }
            return ofields;
        }
        private static IDictionary<string, FieldValue> GetWixFields()
        {
            IDictionary<string, FieldValue> ofields = new Dictionary<string, FieldValue>();
            ofields.Add(new KeyValuePair<string, FieldValue>("_id",
                       new FieldValue()
                       {
                           DisplayName = "_id",
                           QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                           Type = "text"
                       })
                );
            ofields.Add(new KeyValuePair<string, FieldValue>("_owner",
                      new FieldValue()
                      {
                          DisplayName = "_owner",
                          QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                          Type = "text"
                      })
               );

            ofields.Add(new KeyValuePair<string, FieldValue>("Casetitle",
                      new FieldValue()
                      {
                          DisplayName = "Casetitle",
                          QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                          Type = "text"
                      })
               );
            ofields.Add(new KeyValuePair<string, FieldValue>("Casetype",
                     new FieldValue()
                     {
                         DisplayName = "Casetype",
                         QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                         Type = "text"
                     })
              );
            ofields.Add(new KeyValuePair<string, FieldValue>("Casestatus",
                    new FieldValue()
                    {
                        DisplayName = "Casestatus",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("Currentactivityid",
                   new FieldValue()
                   {
                       DisplayName = "Currentactivityid",
                       QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                       Type = "text"
                   })
            );
            ofields.Add(new KeyValuePair<string, FieldValue>("Currentactionid",
                   new FieldValue()
                   {
                       DisplayName = "Currentactionid",
                       QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                       Type = "text"
                   })
            );
            ofields.Add(new KeyValuePair<string, FieldValue>("Casedescription",
                   new FieldValue()
                   {
                       DisplayName = "Casedescription",
                       QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                       Type = "text"
                   })
            );
            ofields.Add(new KeyValuePair<string, FieldValue>("Createdate",
                    new FieldValue()
                    {
                        DisplayName = "Createdate",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("Createuser",
                    new FieldValue()
                    {
                        DisplayName = "Createuser",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("Updatedate",
                   new FieldValue()
                   {
                       DisplayName = "Updatedate",
                       QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                       Type = "text"
                   })
            );
            ofields.Add(new KeyValuePair<string, FieldValue>("Updateuser",
                   new FieldValue()
                   {
                       DisplayName = "Updateuser",
                       QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                       Type = "text"
                   })
            );

            return ofields;
        }
        
        private static IDictionary<string, FieldValue> GetTenantFields()
        {
            IDictionary<string, FieldValue> ofields = GetWixFields();
            ofields.Add(new KeyValuePair<string, FieldValue>("tenantname",
                    new FieldValue()
                    {
                        DisplayName = "Tenant Name",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("tenantdesc",
                    new FieldValue()
                    {
                        DisplayName = "Tenant Description",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            
            return ofields;
        }
        private static IDictionary<string, FieldValue> GetCaseTypes()
        {
            IDictionary<string, FieldValue> ofields = GetWixFields();
            ofields.Add(new KeyValuePair<string, FieldValue>("tenantid",
                    new FieldValue()
                    {
                        DisplayName = "Tenant Id",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("casetypename",
                    new FieldValue()
                    {
                        DisplayName = "Case Type Description",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("casetypedesc",
                  new FieldValue()
                  {
                      DisplayName = "Case Type Description",
                      QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                      Type = "text"
                  })
           );
            return ofields;
        }

        private static IDictionary<string, FieldValue> GetCaseTypeFields()
        {
            IDictionary<string, FieldValue> ofields = GetWixFields();
            ofields.Add(new KeyValuePair<string, FieldValue>("tenantid",
                    new FieldValue()
                    {
                        DisplayName = "Tenant Id",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("casetypeid",
                    new FieldValue()
                    {
                        DisplayName = "Case Type Id",
                        QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                        Type = "text"
                    })
             );
            ofields.Add(new KeyValuePair<string, FieldValue>("fieldname",
                  new FieldValue()
                  {
                      DisplayName = "Field Name",
                      QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                      Type = "text"
                  })
           );
            ofields.Add(new KeyValuePair<string, FieldValue>("fielddesc",
                 new FieldValue()
                 {
                     DisplayName = "Field Description",
                     QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                     Type = "text"
                 })
          ); 
            ofields.Add(new KeyValuePair<string, FieldValue>("fieldtype",
                  new FieldValue()
                  {
                      DisplayName = "Field Type",
                      QueryOperators = new string[] { "eq", "lt", "gt", "hasSome", "and", "lte", "gte", "or", "not", "ne", "startsWith", "endsWith" },
                      Type = "text"
                  })
           );
            return ofields;
        }
      
    }
}
