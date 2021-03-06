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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Eyedia.Aarbac.Framework
{
    [DataContract]
    public class RbacColumn
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public RbacDataTypes DataType { get; set; }

        [DataMember]
        public RbacDBOperations AllowedOperations { get; set; }

        public RbacColumn(string name, RbacDataTypes dataType,
            bool create = false, bool read = false, bool update = false)
        {
            this.Name = name;
            this.DataType = dataType;
            //this.IsFilterColumn = isFilterColumn;
            this.AllowedOperations = Rbac.ParseOperations(create, read, update, false);
        }

        public RbacColumn(string name, string dataType,string create = "False", string read = "False", string update = "False")
        {
            switch (dataType)
            {
                case "numeric":
                    dataType = "Decimal";
                    break;
            }
            this.Name = name;
            this.DataType = (RbacDataTypes)Enum.Parse(typeof(RbacDataTypes), dataType, true);            
            this.AllowedOperations = Rbac.ParseOperations(create, read, update, "False");
        }

        public XmlNode ToXml(XmlDocument doc)
        {           
            XmlNode columnNode = doc.CreateElement("Column");
            XmlAttribute cName = doc.CreateAttribute("Name");
            cName.Value = Name;
            columnNode.Attributes.Append(cName);

            XmlAttribute type = doc.CreateAttribute("Type");
            type.Value = DataType.ToString();
            columnNode.Attributes.Append(type);

            XmlAttribute create = doc.CreateAttribute("Create");
            create.Value = AllowedOperations.CanCreate().ToString();
            columnNode.Attributes.Append(create);

            XmlAttribute read = doc.CreateAttribute("Read");
            read.Value = AllowedOperations.CanRead().ToString();
            columnNode.Attributes.Append(read);

            XmlAttribute update = doc.CreateAttribute("Update");
            update.Value = AllowedOperations.CanUpdate().ToString();
            columnNode.Attributes.Append(update);

            return columnNode;
        }

    }
}


