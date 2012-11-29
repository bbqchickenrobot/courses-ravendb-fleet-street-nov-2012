﻿using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Eastwind.Models;
using System;

namespace Eastwind.Controllers
{
    public class HomeController : RavenController
    {
		public ActionResult Measure()
		{
			Session.Store(new User
				{
					ZipCode = 13
				});
			Session.SaveChanges();

			var sp = Stopwatch.StartNew();
			while (DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
			{
			}

			return Json(sp.ElapsedMilliseconds);

		}

        public ActionResult Index()
        {
            var users = Session.Query<User>()
                               .Take(3)
                               .ToList();

            return Json(new
                {
                    users.Count,
                    Results = users
                });
		}

		public ActionResult Create()
		{
			for (int i = 0; i < 10; i++)
			{
				var entity = new User
				{
					GivenName = DateTime.Now.ToString()
				};
				Session.Store(entity);
			}

			return Json("ok");
		}

		public ActionResult Show(string DocID)
		{
			var load = Session.Load<User>(DocID);
			load.NationalID = DateTime.Now.ToString();
			return Json(load);
		}

		public ActionResult Search(string surname)
		{
			return Json(
				Session.Query<User>()
				       .Where(x => x.Surname == surname)
				       .ToList()
				);
        }

        public ActionResult Add()
        {
            var dude = new User
                {
                    Id = string.Format("users/{0}", new Random().NextDouble()),
                    City = "Los Angeles",
                    Company = "ummm....",
                    EmailAddress = "thedude@lebowski.com",
                    Gender = "dude",
                    GivenName = "Jeff",
                    Surname = "Lebowski",
                    Username = "thedude"
                };

            Session.Store(dude);
            return RedirectToAction("Show", new { docId = dude.Id });
        }

        public ActionResult Show(object usr)
        {
            return Json(usr);
        }

        public ActionResult Query(string p, string f)
        {
            var result = new List<User>();
            switch (f)
            {
                case "usr":
                    result = Session.Query<User>().Where(x => x.GivenName == p).ToList();
                    break;
                case "sur":
                    result = Session.Query<User>().Where(x => x.Surname == p).ToList();
                    break;
                case "city":
                    result = Session.Query<User>().Where(x => x.City == p).ToList();
                    break;
                case "zip":
                    int zzip;
                    if (Int32.TryParse(p, out zzip))
                    {
                        result = Session.Query<User>().Where(x => x.ZipCode == zzip).ToList();
                    }
                    break;

                case "state":
                    result = Session.Query<User>().Where(x => x.State== p).ToList();
                    break;

            }
            return Json(new { result.Count, Results = result });
        }
    }
}