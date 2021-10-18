using MBADCases.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MBADCases.Models;
using MBADCases.Authentication;
using MongoDB.Bson;
using System;
using MongoDB.Bson.Serialization;
using Microsoft.AspNetCore.Http;
namespace MBADCases.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [BasicAuthFilter()]
    public class VaultController : ControllerBase
    {
        private readonly VaultService _vaultservice;
        public VaultController(VaultService adapterser)
        {
            _vaultservice = adapterser;
        }
        [HttpGet("{id:length(24)}", Name = "GetVault")]
        public IActionResult Get(string id)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _vaultservice.Gettenant(tenantid);

                VaultResponse ovaultresp = _vaultservice.Get(id);
                oms = _vaultservice.SetMessage(id, id, "GET", "200", "Case type Search", usrid, null);
                if (ovaultresp == null)
                {
                    ovaultresp = new VaultResponse();
                    oms = _vaultservice.SetMessage(ovaultresp._id, id, "GET", "400", "Not found", usrid, null);
                    ovaultresp.Message = new MessageResponse() { Messagecode = oms.Messagecode,  Messageype = oms.Messageype, _id = oms._id };
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ovaultresp);
                }
                else
                {
                    oms = _vaultservice.SetMessage(ovaultresp._id, id, "GET", "200", "Case type Search by name", usrid, null);
                    ovaultresp.Message = new MessageResponse() { Messagecode = oms.Messagecode,  Messageype = oms.Messageype, _id = oms._id };
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ovaultresp);
                }
            }
            catch (Exception ex)
            {
                CaseType ocase = new CaseType();
                ocase._id = id;
                oms = _vaultservice.SetMessage(id, id, "GET", "501", "Case Type Search", usrid, ex);

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseTypeResponse(ocase, oms));
            }
        }

        [HttpGet("{name}", Name = "GetVaultByName")]
        public IActionResult GetByName(string name)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _vaultservice.Gettenant(tenantid);

                if (name.ToLower() == "all")
                {
                   
                    List<Vault> ocase = _vaultservice.Searchvault(name);
                   
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                }
                else
                {
                    VaultResponse ocase = _vaultservice.GetByName(name);
                    if (ocase == null)
                    {
                        ocase = new VaultResponse();
                        oms = _vaultservice.SetMessage(ocase._id, name, "GET", "400", "Not found", usrid, null);
                        ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound, ocase);
                    }
                    else
                    {
                        oms = _vaultservice.SetMessage(ocase._id, name, "GET", "200", "Case type Search by name", usrid, null);
                        ocase.Message = new MessageResponse() { Messagecode = oms.Messagecode, Messageype = oms.Messageype, _id = oms._id };
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                    }
                }
              

             

            }
            catch (Exception ex)
            {
                VaultResponse ocaset = new VaultResponse();

                oms = _vaultservice.SetMessage(name, name, "GET", "501", "Case Type Search", usrid, ex);
                ocaset.Message = new MessageResponse() { Messagecode = oms.Messagecode,  Messageype = oms.Messageype, _id = oms._id };
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, ocaset);
            }
        }

        [HttpPost("{id:length(24)}", Name = "UpdateVault")]
        public IActionResult Post(string id, Vault ovault)
        {
            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            //string id = ocase._id;
            ovault._id = id;
            try
            {
                //make sure the id belongs to this tenant
                
                if (ovault.Name == null || ovault.Name == "")
                {
                    ovault.Name = "Vault_" + helperservice.RandomString(5, false);
                }

                ovault.Macroname = "@VAULT|" + ovault.Name + "@";
                _vaultservice.Gettenant(tenantid);

                _vaultservice.Update(id, ovault);
                oms = _vaultservice.SetMessage(id, null, "POST", "UPDATE", "Case type update", usrid, null);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseResponse(ovault._id, oms));
            }
            catch (Exception ex)
            {
                oms = _vaultservice.SetMessage(id, null, "POST", "UPDATE", "Case type update", usrid, ex);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ovault._id, oms));
            }
        }

        [HttpPut()]
        public IActionResult Put(Vault ovault)
        {
            string sj = ovault.ToJson();

            Message oms;
            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                VaultResponse oretcase;
                _vaultservice.Gettenant(tenantid);
                if (ovault.Name == null || ovault.Name == "")
                {
                    ovault.Name = "Vault_" + helperservice.RandomString(5, false);
                }
                else
                {
                    //check if name is unique
                    if ((oretcase=_vaultservice.GetByName(ovault.Name) )!= null) {
                        oms = _vaultservice.SetMessage(oretcase._id, sj, "PUT", "200", "Case insert", usrid, null);
                        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseResponse(oretcase._id, oms));
                    };
                }
                Vault ovlt;
                ovault.Macroname = "@VAULT|" + ovault.Name + "@";
                ovlt = _vaultservice.Create(ovault);
                oms = _vaultservice.SetMessage(ovlt._id, sj, "PUT", "200", "Case insert", usrid, null);

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, new CaseResponse(ovlt._id, oms));
            }
            catch (Exception ex)
            {
                oms = _vaultservice.SetMessage("", sj, "PUT", "", "Case insert", usrid, ex);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status417ExpectationFailed, new CaseResponse(ovault._id, oms));

            }

        }
        [MapToApiVersion("1.0")]
        [HttpGet("/all", Name = "GetVaultbyfilter")]
        public IActionResult Search(string filter)
        {

            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");
            try
            {
                _vaultservice.Gettenant(tenantid);

                List<Vault> ocase = _vaultservice.Searchvault(filter);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);
                
            }
            catch  
            {
             
                throw;
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {


            var usrid = HttpContext.Session.GetString("mbaduserid");
            var tenantid = HttpContext.Session.GetString("mbadtanent");

            try
            {
                _vaultservice.Gettenant(tenantid);

                _vaultservice.Remove(id);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "");
            }
            catch  
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError, "");

            }
        }
    }
}
