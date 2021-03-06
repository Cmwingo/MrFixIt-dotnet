﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrFixIt.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MrFixIt.Controllers
{
    public class JobsController : Controller
    {
        private MrFixItContext db = new MrFixItContext();

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(db.Jobs.Include(i => i.Worker).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Job job)
        {
            db.Jobs.Add(job);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Claim(int id)
        {
            var thisItem = db.Jobs.FirstOrDefault(items => items.JobId == id);
            return View(thisItem);
        }

        [HttpPost]
        public IActionResult Claim(Job job)
        {
            job.Worker = db.Workers.FirstOrDefault(i => i.UserName == User.Identity.Name);
            job.Worker.Avaliable = false;
            db.Entry(job).State = EntityState.Modified;
            db.SaveChanges();
            return Json(job);
        }

        public IActionResult Detail(int id)
        {
            var thisItem = db.Jobs.FirstOrDefault(items => items.JobId == id);

            return View(thisItem);
        }

        [HttpPost]
        public IActionResult BeginJob(Job job)
        {
            if (job.Pending == false)
            {
                job.Pending = true;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return Json(job);
            }
            else
            {
                return Json(job);
            }
        }

        [HttpPost]
        public IActionResult CompleteJob(Job job)
        {
            if (job.Completed == false)
            {
                job.Completed = true;
                Worker thisWorker = job.FindWorker(User.Identity.Name);
                thisWorker.Avaliable = true;
                db.Entry(job).State = EntityState.Modified;
                db.Entry(thisWorker).State = EntityState.Modified;
                db.SaveChanges();
                return Json(job);
            }
            else
            {
                return Json(job);
            }
        }

    }
}
