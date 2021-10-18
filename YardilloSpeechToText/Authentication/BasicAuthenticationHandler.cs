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

namespace MBADCases.Authentication
{
    public class BasicAuthenticationHandler : Attribute, IAuthorizationFilter
    {
        private readonly string _realm = string.Empty;


        public BasicAuthenticationHandler()
        {

 
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            Message omess = new Message();
         
            try
            {
                bool allpass = true;

                string srapidsecretkey = context.HttpContext.Request.Headers["X-RapidAPI-Proxy-Secret"];
                string rapiduserid = context.HttpContext.Request.Headers["X-RapidAPI-User"];
                var ssubs = context.HttpContext.Request.Headers["X-RapidAPI-Subscription"];
                string sauthsrc = context.HttpContext.Request.Headers["Y-Auth-Src"];
                string srapidapikey = context.HttpContext.Request.Headers["x-rapidapi-key"];
                string syauthuseridopt = context.HttpContext.Request.Headers["Y-Auth-userid"];

                omess.Callertype = "Headers";
                omess.Messagecode = "yardillo";
                string sheaders = Newtonsoft.Json.JsonConvert.SerializeObject(context.HttpContext.Request.Headers);
                omess.Headerrequest = sheaders;
                omess.Userid = rapiduserid;
                omess.YAuthSource = sauthsrc;

                if (srapidsecretkey != "1f863a60-f3b6-11eb-bc3e-c3f329db9ee7" && srapidsecretkey != "6acc1280-fde1-11eb-b480-3f057f12dc26" && srapidsecretkey != "ade9f2f0-fe3e-11eb-8e8b-29cf15887162" && srapidsecretkey != "d602ee50-2f9d-11ec-9121-f55b1f38643f")
                { allpass = false; omess.Messageype = "Unauthorized"; omess.Messagecode = "00001"; }

               
                if (allpass) { return; }

                if (allpass)
                {

                    string authHeader = context.HttpContext.Request.Headers["Authorization"];
                    if (authHeader != null)
                    {
                        var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                        if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            var credentials = Encoding.UTF8
                                                .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                                .Split(':', 2);
                            if (credentials.Length == 2)
                            {
                                //if (IsAuthorized(context, credentials[0], credentials[1]))
                                //{
                                //    return;
                                context.HttpContext.Session.SetString("mbadtanent", credentials[0]);

                                return;
                                //}
                            }
                        }
                    }
                }


                ReturnUnauthorizedResult(context);
            }
            catch (FormatException e)
            {
                omess.MessageDesc = "Unabel to validate user" + e.ToString();
                // _messagemaster.InsertOneAsync(omess);
                ReturnUnauthorizedResult(context);
            }
            finally
            {
                
            }
        }

        //public bool IsAuthorized(AuthorizationFilterContext context, string username, string password)
        //{
        //    var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        //    return true;// userService.IsValidUser(username, password);
        //}

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }
    }
     
}
