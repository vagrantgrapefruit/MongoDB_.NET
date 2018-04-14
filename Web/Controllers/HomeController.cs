using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Web.Models;
using Web.Connection;
using System.Configuration;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        //DBConnection<TestModel> dBConnection = new DBConnection<TestModel>(ConfigurationManager.AppSettings["MongoDb"] , "test", "test");
        DBConnection<TestModel> dBConnection = new DBConnection<TestModel>(ConfigurationManager.AppSettings["MongoDbTelnet"], "test", "test");
        
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="departmentname"></param>
        /// <param name="statu"></param>
        /// <returns></returns>
        public string GetList(int limit, int offset, string departmentname, string statu)
        {
            string jsondata = null;
            try
            {
                List<TestModel> modelList = dBConnection.Select(a => a.Name!=null);
                var total = modelList.Count;
                var rows = modelList.Skip(offset).Take(limit).ToList();
                
                jsondata = js.Serialize(new { total = total, rows = rows });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return jsondata;


        }

        /// <summary>
        /// 删除选中的一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult DeleteOne(TestModel model)
        {
            if(model!=null)
            {
                var tag= dBConnection.DeleteBypredicate(a=>a.Name ==model.Name);
                return Json(new {
                     tag = tag
                });
            }
            else return Json(new
            {
                tag = false
            });
        }


        public JsonResult AddOne(TestModel model)
        {
            if (model != null)
            {
                var tag = dBConnection.Insert(model);
                return Json(new
                {
                    tag = tag
                });
            }
            else return Json(new
            {
                tag = false
            });
        }
    }
}