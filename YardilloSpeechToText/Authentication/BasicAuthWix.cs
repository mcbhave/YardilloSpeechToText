using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using MBADCases.Services;
using MBADCases.Models;
using MongoDB.Driver;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Linq;

namespace MBADCases.Authentication
{
    public class BasicAuthWix : Attribute, IAuthorizationFilter
    {
        private readonly string _realm = string.Empty;
        public BasicAuthWix(string realm)
        {
            _realm = realm;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
           
            MongoClient _client;
            IMongoDatabase MBADDatabase;
            IMongoCollection<Message> _messagemaster;
            Message omess = new Message();
            _client = new MongoClient("mongodb://yardilloadmin:1pkGpqdqHV42AvOD@cluster0-shard-00-00.tj6lt.mongodb.net:27017,cluster0-shard-00-01.tj6lt.mongodb.net:27017,cluster0-shard-00-02.tj6lt.mongodb.net:27017/yardillo_dev?ssl=true&replicaSet=atlas-d5jcxa-shard-0&authSource=admin&retryWrites=true&w=majority");
            MBADDatabase = _client.GetDatabase("YARDILLO");
            _messagemaster = MBADDatabase.GetCollection<Message>("WIXlogins");

            try
            {
               
                bool allpass = true;
                 
                omess.Callertype = "Headers";
                omess.Messagecode = "wix";
                string sheaders = Newtonsoft.Json.JsonConvert.SerializeObject(context.HttpContext.Request.Headers);
                omess.Headerrequest = sheaders;
                _messagemaster.InsertOneAsync(omess);

                if (allpass)
                {
                    //reject any other host
                    string xrapidhost = context.HttpContext.Request.Headers["Host"];
                    if (xrapidhost != "yardillo.azurewebsites.net" && xrapidhost!= "localhost:44346")
                    { allpass = false; omess.MessageDesc = "Invalid host"; }

                }

                if (allpass)
                {

                    var syncIOFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
                    if (syncIOFeature != null)
                    {
                        syncIOFeature.AllowSynchronousIO = true;

                        var req = context.HttpContext.Request;

                        req.EnableBuffering();

                        // read the body here as a workarond for the JSON parser disposing the stream
                        if (req.Body.CanSeek)
                        {
                            req.Body.Seek(0, SeekOrigin.Begin);

                            // if body (stream) can seek, we can read the body to a string for logging purposes
                            using (var reader = new StreamReader(
                                 req.Body,
                                 encoding: Encoding.UTF8,
                                 detectEncodingFromByteOrderMarks: false,
                                 bufferSize: 8192,
                                 leaveOpen: true))
                            {
                                var jsonString = reader.ReadToEnd();

                                if (jsonString != null && jsonString != "")
                                {
                                    omess.Callerrequest = jsonString;
                                    _messagemaster.InsertOneAsync(omess);

                                    wixreqC owixreq = Newtonsoft.Json.JsonConvert.DeserializeObject<wixreqC>(jsonString);
                                    if (owixreq != null)
                                    {
                                        string stenant = owixreq.requestContext.settings.Yauthtenantname;
                                        if (stenant != null) { 
                                        string rapiduserid = owixreq.requestContext.memberId;
                                        //find the tenant id

                                        //initially wix only sends installationId, use that as memeber and pass
                                        string installationId = owixreq.requestContext.installationId;
                                        string role = owixreq.requestContext.role;
                                        string stype = "Member";
                                        if (rapiduserid==null && installationId != null)
                                        {
                                            rapiduserid = installationId;
                                            stype = "Installation";
                                        }                                        
                                        
                                        IMongoCollection<Tenant> _tenantcoll;
                                        _tenantcoll = MBADDatabase.GetCollection<Tenant>("Tenants");
                                        Tenant ot = _tenantcoll.Find(t => t.Tenantname.ToUpper() == stenant.ToUpper()).FirstOrDefault();
                                        if (ot != null)
                                        { 
                                            context.HttpContext.Session.SetString("mbadtanent", ot.Tenantname);

                                            //all good
                                            IMongoCollection<TenantUser> _tenantusercoll;
                                            _tenantusercoll = MBADDatabase.GetCollection<TenantUser>("TenantUsers");
                                            //find if user exists
                                            TenantUser ousr;
                                            ousr = _tenantusercoll.Find(t => t.Userid.ToUpper() == rapiduserid.ToUpper() && t.Tenantname.ToUpper() == ot.Tenantname.ToUpper()).FirstOrDefault();
                                            if (ousr == null)
                                            {
                                                ousr = new TenantUser();
                                                ousr.Userid = rapiduserid;
                                                ousr.Tenantname = ot.Tenantname;
                                                ousr.Createdate = DateTime.UtcNow.ToString();
                                                ousr.Createuserid = rapiduserid;
                                                ousr.Source = "Wiki";
                                                ousr.Type = stype;
                                                ousr.Role = role;
                                                ousr.RapidAPIkey = ot.Rapidapikey;
                                                ousr.Installationid = installationId;
                                                _tenantusercoll.InsertOne(ousr);
                                            }

                                            context.HttpContext.Session.SetString("mbaduserid", rapiduserid);
                                             
                                        }
                                        else
                                        {
                                            omess.MessageDesc = "Invalid Tenant";
                                                omess.Messageype = "Unauthorized";
                                                allpass = false;
                                        }
                                    }
                                    else
                                    {
                                        omess.MessageDesc = "Invalid Body Request";
                                            omess.Messageype = "Unauthorized";
                                            allpass = false;
                                    }
                                    }
                                    else
                                    {
                                        omess.MessageDesc = "Invalid Body Request";
                                        omess.Messageype = "Unauthorized";
                                        allpass = false;
                                    }
                                }
                                // store into the HTTP context Items["request_body"]
                                //context.HttpContext.Items.Add("request_body", jsonString);
                            }

                            // go back to beginning so json reader get's the whole thing
                            req.Body.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            omess.Callerrequest = "No Body Request";
                        }
                    }
                }
             
                if (allpass) { return; }


                omess.Callertype = "Auth";
                omess.Messagecode = "wix";
              
                _messagemaster.InsertOneAsync(omess);

                ReturnUnauthorizedResult(context);
            }
            catch (FormatException e)
            {
                omess.MessageDesc = "Unabel to validate user" + e.ToString();
                _messagemaster.InsertOneAsync(omess);
                ReturnUnauthorizedResult(context);
            }
        }
        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }
        public class Settings
        {
            public string Yauthtenantname { get; set; }
        }

        public class RequestContext
        {
            public Settings settings { get; set; }
            public string instanceId { get; set; }
            public string installationId { get; set; }
            public string memberId { get; set; }
            public string role { get; set; }
        }

        public class wixreqC
        {
            public RequestContext requestContext { get; set; }
        }
    }
   
}
