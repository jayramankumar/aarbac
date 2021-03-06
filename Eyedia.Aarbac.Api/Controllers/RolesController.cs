#region Copyright Notice
/* Copyright (c) 2017, Deb'jyoti Das - debjyoti@debjyoti.com
 All rights reserved.
 Redistribution and use in source and binary forms, with or without
 modification, are not permitted.Neither the name of the 
 'Deb'jyoti Das' nor the names of its contributors may be used 
 to endorse or promote products derived from this software without 
 specific prior written permission.
 THIS SOFTWARE IS PROVIDED BY Deb'jyoti Das 'AS IS' AND ANY
 EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 DISCLAIMED. IN NO EVENT SHALL Debjyoti OR Deb'jyoti OR Debojyoti Das OR Eyedia BE LIABLE FOR ANY
 DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#region Developer Information
/*
Author  - Debjyoti Das (debjyoti@debjyoti.com)
Created - 11/12/2017 3:31:31 PM
Description  - 
Modified By - 
Description  - 
*/
#endregion Developer Information

#endregion Copyright Notice

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Eyedia.Aarbac.Framework;

namespace Eyedia.Aarbac.Api.Controllers
{
    [RoutePrefix("api/roles")]
    public class RolesController : ApiController
    {
        [HttpGet]
        [Route]
        [ActionName("getall")]
        public List<RbacRoleWeb> Get()
        {
            List<RbacRole> roles = Rbac.GetRoles();
            List<RbacRoleWeb> rolesW = new List<RbacRoleWeb>();

            foreach (var role in roles)
            {
                rolesW.Add(new RbacRoleWeb(role));
            }

            return rolesW;
        }

        [ActionName("getbyid")]
        public RbacRoleWeb Get(int id)
        {
            RbacRole role = Rbac.GetRole(id);
            if (role != null)
            {
                role.ParseMetaData();
                return new RbacRoleWeb(role);                
            }
            return null;
        }

        [ActionName("getbyname")]
        public RbacRoleWeb Get(string name)
        {
            RbacRole role = Rbac.GetRole(name);
            if (role != null)
            {
                role.ParseMetaData();
                return new RbacRoleWeb(role);
            }
            return null;
        }

        [HttpPost]
        [Route]
        public RbacRole Post([FromBody]RbacRoleWeb role)
        {
            return Rbac.Save(role);
        }

        [HttpPut]
        [Route]
        public RbacRole Put(int id, [FromBody]RbacRoleWeb role)
        {
            return Rbac.Save(role);
        }

        [HttpDelete]
        [Route]
        public void Delete(int id)
        {
        }
    }
}


