using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace Web.Models
{
    public class TestModel
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public double Age { get; set; }
        public string extra { get; set; }
    }
}