﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Forager.Controllers
{
    public class ErrorController : ApplicationController
    {
        //
        // GET: /Error/General

        public ActionResult General()
        {
            return View();
        }

        //
        // GET: /Error/Http404

        public ActionResult Http404()
        {
            return View();
        }
    }
}
