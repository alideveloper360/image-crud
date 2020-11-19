using Rotativa;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication10.Models;

namespace WebApplication10.Controllers
{
    public class HomeController : Controller
    {
        F3Entities f3 = new F3Entities();
        // GET: Home
        public ActionResult Index()
        {
            return View(f3.tbl3.ToList());
        }
        public ActionResult ExportPDF(int? id)
        {
            return new ActionAsPdf("Details",id)
            {
                FileName = Server.MapPath("~/Content/ListProducts.pdf")
            };
        }
        public ActionResult PrintPartialViewToPdf(int id)
        {
            using (F3Entities db = new F3Entities())
            {
                tbl3 customer = db.tbl3.FirstOrDefault(c => c.id == id);

                var report = new ViewAsPdf("~/Views/Home/Details.cshtml", customer);
                return report;
            }

        }
        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbl_img = f3.tbl3.Find(id);
            //Session["imgpath"] = tbl_img.img;
            if (tbl_img == null)
            {
                return HttpNotFound();
            }
            return View(tbl_img);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file,tbl3 tbl)
        {
            try
            {
                String fileName = Path.GetFileName(file.FileName);
                string _fileName = DateTime.Now.ToString("yymmssfff") + fileName;
                string extention = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), _fileName);
                tbl.img = "~/images/" + _fileName;
                if(extention.ToLower()==".jpg" || extention.ToLower()==".jpeg"||extention.ToLower()==".png")
                {
                    if(file.ContentLength<=1000000)
                    {
                        f3.tbl3.Add(tbl);
                        if(f3.SaveChanges()>0)
                        {
                            file.SaveAs(path);
                            ModelState.Clear();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tbl_img = f3.tbl3.Find(id);
            Session["imgpath"] = tbl_img.img;
            if(tbl_img==null)
            {
                return HttpNotFound();
            }
            return View(tbl_img);
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file,tbl3 tbl)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if(file!=null)
                    {
                        String fileName = Path.GetFileName(file.FileName);
                        string _fileName = DateTime.Now.ToString("yymmssfff") + fileName;
                        string extention = Path.GetExtension(file.FileName);
                        string path = Path.Combine(Server.MapPath("~/images/"), _fileName);
                        tbl.img = "~/images/" + _fileName;
                        if (extention.ToLower() == ".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                        {
                            if (file.ContentLength <= 1000000)
                            {
                                f3.Entry(tbl).State = EntityState.Modified;
                                string oldimgpath = Request.MapPath(Session["imgpath"].ToString());

                                //f3.tbl3.Add(tbl);
                                if (f3.SaveChanges() > 0)
                                {
                                    file.SaveAs(path);
                                    if(System.IO.File.Exists(oldimgpath))
                                    {
                                        System.IO.File.Delete(oldimgpath);
                                    }
                                    //ModelState.Clear();
                                }
                            }
                        }
                    }
                }
                else
                {
                    tbl.img = Session["imgpath"].ToString();
                    f3.Entry(tbl).State = EntityState.Modified;
                    if(f3.SaveChanges()>0)
                    {
                        return RedirectToAction("Index");
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
