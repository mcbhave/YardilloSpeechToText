using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MBADCases.Authentication;
using MBADCases.Models;
using MBADCases.Services;
using static MBADCases.Models.WixDB;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace MBADCases.Controllers
{
    [Route("data")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/data")]
    [BasicAuthWix("wix")]
    public class WixdataController : ControllerBase
    {
        private readonly CaseService _cases;
        public WixdataController(CaseService cases)
        {
            _cases = cases;
        }
        [Route("insert")]
        [Route("insert/{id?}")]
        [HttpPost]
        public IActionResult data(object  oid)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
                srequest= oid.ToString();
                WixDB.data id = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(oid.ToString());

                // var oitm =  Newtonsoft.Json.JsonConvert.DeserializeObject<Case>(js);


                Case ocase = new Case();
                ocase.Casetype = id.collectionName;
                // ocase.item = id.itemId;
                _cases.Create(ocase);
                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, null);

            }
            catch (Exception ex)
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                throw;
            }
            finally
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
            }
        }

        [Route("get")]
        [Route("get/{id?}")]
        [HttpPost]
        public IActionResult getitem(object sid)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
                _cases.Gettenant(tenantid);

                srequest = sid.ToString();
               WixDB.data id = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(sid.ToString());
                                               
                DataItem<WixCase> oi = new DataItem<WixCase>();
                
                  var  ocase = new Case() { Casetype = id.collectionName, itemId=id.itemId };
                   var c = _cases.Create(ocase);
                    WixCase owix = new WixCase();
                    owix.casedescription = c.Casedescription;
                    owix._id = c.itemId;
                    owix._owner = c.Createuser;
                    owix.casestatus = c.Casestatus;
                    owix.casetitle = c.Casetitle;
                    owix.casetype = c.Casetype;
                    owix.createdate = c.Createdate;
                    owix.createuser = c.Createuser;
                    owix.currentactionid = c.Currentactionid;
                    owix.currentactivityid = c.Currentactivityid;
                    owix.updatedate = c.Updatedate;
                    owix.updateuser = c.Updateuser;
                    oi.item = owix;
                
               
                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(oi);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, oi);

                  
 
            }
            catch (Exception ex)
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix datagetItem", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                throw;
            }
            finally
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix datagetItem", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
            }


        }
        [Route("find")]
        [Route("find/{id?}")]
        [HttpPost]
        public IActionResult finditem(WixDB.data id)
        {
             
             
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
                _cases.Gettenant(tenantid);
                string js = Newtonsoft.Json.JsonConvert.SerializeObject(id.item);
                var oitm = Newtonsoft.Json.JsonConvert.DeserializeObject<Case>(js);
                DataItem<WixCase> oi = new DataItem<WixCase>();
                // List<FindItems<JObject>> od = new List<FindItems<JObject>>();
                List<JObject> o = new List<JObject>();
                foreach (Case c in _cases.SearchcasesfromRapidapi("Casetype=" + id.collectionName))
                {
                    WixCase ocase = new WixCase();
                    //job.Add("Casedescription", c.Casedescription);

                    ocase.casedescription = c.Casedescription;
                    ocase._id = c._id;
                    ocase._owner = c.Createuser;
                    ocase.casestatus = c.Casestatus;
                    ocase.casetitle = c.Casetitle;
                    ocase.casetype = c.Casetype;
                    ocase.createdate = c.Createdate;
                    ocase.createuser = c.Createuser;
                    ocase.currentactionid = c.Currentactionid;
                    ocase.currentactivityid = c.Currentactivityid;
                    ocase.updatedate = c.Updatedate;
                    ocase.updateuser = c.Updateuser;

                    string oitem = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);

                    JObject job = JObject.Parse(oitem);
                    foreach (Casefield f in c.Fields)
                    {
                        job.Add(f.Fieldid.ToLower(), f.Value);
                    }
                    
                    o.Add(job);
                }

                FindItems<JObject> olistdata = new FindItems<JObject>();
                // MBADCases.Data.TenantData otendata = new Data.TenantData(id.collectionName);
                //List<Tenant> listtn = otendata.getTenants();
                olistdata.items = o;
                olistdata.totalCount = o.Count;
                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(olistdata);

                var retj = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(sresponse);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, olistdata);

            }
            catch (Exception ex)
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data find", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                throw;
            }
            finally
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data find", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
            }
        }

        [Route("update")]
        [Route("update/{id?}")]
        [HttpPost]
        public IActionResult updateitem(object id)
        {
            string usrid = HttpContext.Session.GetString("mbaduserid");
            string tenantid = HttpContext.Session.GetString("mbadtanent");
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
                _cases.Gettenant(tenantid);

                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(id);
                WixDB.data sid = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(srequest);

                string js = Newtonsoft.Json.JsonConvert.SerializeObject(sid.item);
                Case oc = Newtonsoft.Json.JsonConvert.DeserializeObject<Case>(js);

                DataItem<WixCase> oi = new DataItem<WixCase>();

                var ocase = _cases.Searchcases("itemId=" + oc._id).FirstOrDefault();

                if (oc != null)
                {
                     
                    if (ocase.Casetitle != null && ocase.Casetitle.ToLower() != oc.Casetitle.ToLower()) { oc.Casetitle = ocase.Casetitle; }
                    if (ocase.Casenumber != 0 && ocase.Casenumber != oc.Casenumber) { oc.Casenumber = ocase.Casenumber; }
                    if (ocase.Casedescription != null && ocase.Casedescription.ToLower() != oc.Casedescription.ToLower()) { oc.Casedescription = ocase.Casedescription; }
                    if (ocase.Casestatus != null && ocase.Casestatus.ToLower() != oc.Casestatus.ToLower()) { oc.Casestatus = ocase.Casestatus; }
                    if (ocase.Casetype != null && ocase.Casetype.ToLower() != oc.Casetype.ToLower()) { oc.Casetype = ocase.Casetype; }
                   // if (ocase.Currentactionid != null && ocase.Currentactionid.ToLower() != oc.Currentactionid.ToLower()) { oc.Currentactionid = ocase.Currentactionid; }
                   // if (ocase.Currentactivityid != null && ocase.Currentactivityid.ToLower() != oc.Currentactivityid.ToLower()) { oc.Currentactivityid = ocase.Currentactivityid; }
                    Casefield tmpf = null;
                    foreach (Casefield f in ocase.Fields)
                    {
                        tmpf = oc.Fields.Find(fo => fo.Fieldid.ToLower() == f.Fieldid.ToLower());
                        if (tmpf != null)
                        {
                            if (f.Value != null && f.Value.ToLower() != tmpf.Value.ToLower()) { tmpf.Value = f.Value; }
                           
                        }

                    }
                      _cases.Update(ocase._id, ocase);

                    WixCase wcase = new WixCase();
                    //job.Add("Casedescription", c.Casedescription);

                    wcase.casedescription = ocase.Casedescription;
                    wcase._id = ocase.itemId;
                    wcase._owner = ocase.Createuser;
                    wcase.casestatus = ocase.Casestatus;
                    wcase.casetitle = ocase.Casetitle;
                    wcase.casetype = ocase.Casetype;
                    wcase.createdate = ocase.Createdate;
                    wcase.createuser = ocase.Createuser;
                    wcase.currentactionid = ocase.Currentactionid;
                    wcase.currentactivityid = ocase.Currentactivityid;
                    wcase.updatedate = ocase.Updatedate;
                    wcase.updateuser = ocase.Updateuser;

                    oi.item = wcase;
                }

                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(oi);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, oi);
   
            }
            catch (Exception ex)
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix dataUpdate", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString(), Tenantid = tenantid, Userid = usrid });

                throw;
            }
            finally
            {
                _cases.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix dataUpdate", MessageDesc = smessage, Tenantid = tenantid, Userid = usrid });
            }
        }

        [Route("remove")]
        [Route("remove/{id?}")]
        [HttpPost]
        public IActionResult removeitem(object id)
        {
            helperservice.LogWixMessages("removeitem", Newtonsoft.Json.JsonConvert.SerializeObject(id));
            DataItem<item> oi = new DataItem<item>();
            WixDB.item oitem = new WixDB.item();// { _id = Guid.NewGuid().ToString(), _owner = Guid.NewGuid().ToString(), make = "Toyota", model = "Camry", year = 2018, date_added = DateTime.Now.ToString("MM-DD-YYYY HH:mm:ss") };
            oi.item = oitem;
          
            
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, oi);
        }

        [Route("count")]
        [Route("count/{id?}")]
        [HttpPost]
        public IActionResult countitem(object id)
        {
            helperservice.LogWixMessages("countitem", Newtonsoft.Json.JsonConvert.SerializeObject(id));
            DataCount ocount = new DataCount();
            ocount.totalCount = 50;
          

           
            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocount);
        }
        
    }
}
