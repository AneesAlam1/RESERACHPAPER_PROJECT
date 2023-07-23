using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPaperUploadProject.DATABASE;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace RPaperUploadProject.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        DBManager dm = new DBManager();
        public ActionResult Index()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("index", "home");
            }
            

            return View();
        }
        public ActionResult RegDisplay()
        {
                                  
            
            string q = "select * from tbl_registration";
            DataTable dt = dm.ReadBulkData(q);
            string tbl = "<table class='table table-striped'>";
            int i;
            tbl += "<tr>";
            tbl += "<th>Researcher Id</th>";
            tbl += "<th>Researcher Name</th>";
            tbl += "<th>Email</th>";
            tbl += "<th>Contact</th>";
            tbl += "<th>Intro</th>";
            tbl += "<th>City</th>";
            tbl += "<th>Date</th>";
            tbl += "</tr>";
            for (i = 0; i < dt.Rows.Count; i++)
            {
                tbl += "<tr>";
                tbl += "<td>" + dt.Rows[i][0] + "</td>";
                tbl += "<td>" + dt.Rows[i][1] + "</td>";
                tbl += "<td>" + dt.Rows[i][2] + "</td>";
                tbl += "<td>" + dt.Rows[i][3] + "</td>";
                tbl += "<td>" + dt.Rows[i][4] + "</td>";
                tbl += "<td>" + dt.Rows[i][5] + "</td>";
                tbl += "<td>" + dt.Rows[i][6] + "</td>";
                tbl += "</tr>";
            }
            tbl += "</table>";
            ViewBag.table = tbl;
            return View();
        }

        public ActionResult PaperDetails()
        {
            string[] filepaths = Directory.GetFiles(Server.MapPath("~/FILEUPLOAD/"));
            DataTable dtt = new DataTable();
            DataRow dr;
            dtt.Columns.Add("filename");
            foreach (string filepath in filepaths)
            {
                dr = dtt.NewRow();
                dr["filename"] = Path.GetFileName(filepath).ToString();
                dtt.Rows.Add(dr);
            }
            //string ffilepath = Server.MapPath("~/FILEUPLOAD/" + dtt.Rows[0][0]);

            
            string q = "select * from TBL_ARTICLEUPLOAD";
            DataTable dt = dm.ReadBulkData(q);
            string tbl = "<table class='table table-striped'>";
            int i;
            tbl += "<tr>";
            tbl += "<th>Paper ID</th>";
            tbl += "<th>Paper Name</th>";
            tbl += "<th>Author</th>";
            tbl += "<th>Abstract</th>";
            tbl += "<th>Email</th>";
            tbl += "<th>Contact</th>";
            tbl += "<th>Date</th>";
            tbl += "<th>RefCode</th>";
            tbl += "<th>Download</th>";
            tbl += "</tr>";
            for (i = 0; i < dt.Rows.Count; i++)
            {
                tbl += "<tr>";
                tbl += "<td>" + dt.Rows[i][0] + "</td>";
                tbl += "<td>" + dt.Rows[i][1] + "</td>";
                tbl += "<td>" + dt.Rows[i][2] + "</td>";
                tbl += "<td>" + dt.Rows[i][3] + "</td>";
                tbl += "<td>" + dt.Rows[i][5] + "</td>";
                tbl += "<td>" + dt.Rows[i][6] + "</td>";



                string newdt = (dt.Rows[i][7].ToString());
                int kk = newdt.LastIndexOf(' ');
                string dispdate=newdt;
                if (kk != -1)
                    dispdate = newdt.Substring(0, kk);
                
                tbl += "<td>" +dispdate+ "</td>";
                tbl += "<td style='color:red'>" + dt.Rows[i][8] + "</td>";
                

                tbl += "<td><a href='/admin/test?fn="+dt.Rows[i][9]+"'>Download</a></td>";
                tbl += "</tr>";
            }
            tbl += "</table>";
            ViewBag.table = tbl;
            return View();

        }
        public ActionResult LogoutApp()
        {
            Session["uid"] = null;
            return View();
        }
        
        
        public ActionResult test()
        {
            string fname = Request.QueryString["fn"].ToString();

            int x = fname.LastIndexOf(".");
            string ext = fname.Substring(x + 1);
            string filePath = Server.MapPath("~/FILEUPLOAD/" + fname);
            return File(filePath, "application/"+ext, fname);
         }
        public ActionResult EnqDetails()
        {
            string q = "select * from tbl_enquiry";
            DataTable dt = dm.ReadBulkData(q);
            string tbl = "<table class='table table-striped'>";
            int i;
            tbl += "<tr>";
            tbl += "<th>EnquiryNo</th>";
            tbl += "<th>Email</th>";
            tbl += "<th>Enquiry</th>";
            
            tbl += "</tr>";
            for (i = 0; i < dt.Rows.Count; i++)
            {
                tbl += "<tr>";
                tbl += "<td>" + dt.Rows[i][0] + "</td>";
                tbl += "<td>" + dt.Rows[i][1] + "</td>";
                tbl += "<td>" + dt.Rows[i][2] + "</td>";
               
                
                tbl += "</tr>";
            }
            tbl += "</table>";
            ViewBag.table = tbl;
            return View();
            
            return View();
        }
            

            
        }
    }

