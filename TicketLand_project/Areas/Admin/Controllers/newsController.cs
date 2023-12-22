﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TicketLand_project.Models;

namespace TicketLand_project.Areas.Admin.Controllers
{
    public class newsController : Controller
    {
        private QUANLYXEMPHIMEntities db = new QUANLYXEMPHIMEntities();

        // GET: Admin/news
        public ActionResult Index()
        {
            var news = db.news.Include(n => n.movy);
            return View(news.ToList());
        }

        // GET: Admin/news/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: Admin/news/Create
        public ActionResult Create()
        {
            ViewBag.movie_id = new SelectList(db.movies, "movie_id", "movie_name");
            return View();
        }

        public static string GenerateSlug(string title)
        {
            string slug = Regex.Replace(title, @"[^a-zA-Z0-9-]", "-");
            slug = Regex.Replace(slug, @"-{2,}", "-");
            slug = slug.Trim('-').ToLower();
            return slug;
        }

        // POST: Admin/news/Create
        //Xử lý Tạo mới có ajax
        [HttpPost] 
        public JsonResult Create([Bind(Include = "news_id,movie_id,news_title,news_content,news_release")] news news, HttpPostedFileBase news_img)
        {
            if (ModelState.IsValid)
            {
                if (news_img != null && news_img.ContentLength > 0)
                {
                    var slug = GenerateSlug(news.news_title);
                    var posterFileName = Path.GetFileName(news_img.FileName);
                    var extension = Path.GetExtension(posterFileName);
                    var new_file_name = $"{slug}{extension}";
                    var relativePosterPath = "\\Assets\\img\\home\\news\\" + new_file_name;
                    var absolutePosterPath = Path.Combine(Server.MapPath("~/Assets/img/home/news/"), new_file_name);

                    news.news_img = relativePosterPath;

                    // Lưu file vào máy chủ với tên mới
                    using (var fileStream = new FileStream(absolutePosterPath, FileMode.Create))
                    {
                        news_img.InputStream.Seek(0, SeekOrigin.Begin);
                        news_img.InputStream.CopyTo(fileStream);
                    }
                }

                db.news.Add(news);
                db.SaveChanges();
                return Json(new { success = true, message = "Thêm thành công" });
            }

            ViewBag.movie_id = new SelectList(db.movies, "movie_id", "movie_name", news.movie_id);
            return Json(new { success = true, message = "Thêm thất bại!" }); ;
        }
        //Kết thúc 


        // GET: Admin/news/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            ViewBag.movie_id = new SelectList(db.movies, "movie_id", "movie_name", news.movie_id);
            return View(news);
        }

        // POST: Admin/news/Edit/5
        [HttpPost]
 
        public JsonResult Edit([Bind(Include = "news_id,movie_id,news_title,news_content,news_img,news_release")] news news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Chỉnh sửa thành công" });
            }
            ViewBag.movie_id = new SelectList(db.movies, "movie_id", "movie_name", news.movie_id);
            return Json(new { success = false, message = "Chỉnh sửa thất bại!" });
        }

        // GET: Admin/news/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: Admin/news/Delete/5
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            news news = db.news.Find(id);
            if (news != null)
            {
                db.news.Remove(news);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Xóa thất bại!" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}