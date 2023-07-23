using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RPaperUploadProject.DATABASE;
using System.Data;
using System.IO;
using System.Net.Mail;

namespace RPaperUploadProject.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        DBManager dm = new DBManager();
        string q; 
        public ActionResult Index()
        {
           
                
            return View();
        }
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(string tname, string temail, string tcontact, string tintro, string city, string userid, string password )
        {
            string res;
            if (tcontact.Trim().Length != 10)
            {
                res = "Invalid Mobile number";
            }
            else
            {
                String q, q2;
                string dt = DateTime.Now.ToString("dd/MMM/yyyy"); 
                q = "insert into TBL_REGISTRATION values('" + tname + "','" + temail + "','" + tcontact + "','" + tintro + "','"+city
+"','" + dt + "')";
                q2 = "insert into tbl_login values('" + userid + "','" + password + "','" + temail + "')";
                bool r1 = dm.InsertUpdateDelete(q);
                bool r2 = dm.InsertUpdateDelete(q2);

                if (r1 == true && r2==true)
                {
                    res = "Your Registration Is Done";
                }
                else
                {
                    res = "Datbase Error Contact To Admin";
                }
                
            }
            ViewBag.result = res;
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string uname, string pass)
        {
            string q = "select * from tbl_login where userid='" + uname + "' and password='" + pass + "'";
            DataTable dt = new DataTable();
            string res="";
            dt = dm.ReadBulkData(q);
            if (dt.Rows.Count > 0)
            {
                Session["uid"] = uname; 
                if (uname.Equals("Admin"))
                {
                    return (RedirectToAction("index", "admin"));
                }
                else
                {

                    return (RedirectToAction("paperUpload", "Home"));
                }
            }
            else
            {
                res = "Invalid User Name or Password";
            }
            ViewBag.result = res;
            return View();
        }
        public ActionResult PaperUpload()
        {
            if (Session["uid"] != null)
            {
                ViewBag.sid = Session["uid"];
            }
            else
            {
                ViewBag.sid = "Anonymous User";
            }
            return View();
        }
        public void MailSender(HttpPostedFileBase file, string tpapername, string tabstract, string temail, string tcontact, string newfile,string code)
        {
            //Format of the mail Body
            string dt=DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
            string mailbodyformat = "<!DOCTYPE html>" +
                                  "<html>" +
                                  "<body>" +
                                  "<p><b>Dear User,</b> Your Paper is submitted successfully </p>"+
                                  
                                  "<h2 style='color:white;background-color:lightcoral;text-align:center;width:500px'>Confirmation of your Article submission</h2>" +
                                  "<hr>" +
                                  "<h3>Title of the Paper : " + tpapername + "</h3>" +
                                  "<h3>Date of Submission : " + dt + "<h3>" +
                                  "<h3>Reference Code     : " + code + "<h3>";



            string mailTitle = tpapername;
            string subjectmsg = tpapername;
            string bodymsg = mailbodyformat;
            string fromEmailid = "TesingWebSites@gmail.com";
            string toEmailid = temail;
            string mypass = "olmuocxglszkhdpx";

            //Mail Message 
            MailMessage message = new MailMessage(new MailAddress(fromEmailid, mailTitle), new MailAddress(toEmailid));


            message.Subject = subjectmsg;
            message.Body = bodymsg;
            message.IsBodyHtml = true;
            //Mail Attachment


            message.Attachments.Add(new Attachment(file.InputStream, newfile));



            //Server details
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //Credential
            System.Net.NetworkCredential credential = new System.Net.NetworkCredential();

            credential.UserName = fromEmailid;
            credential.Password = mypass;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credential;

            //Send Email
            smtp.Send(message);
            

        }
        [HttpPost]
        public ActionResult PaperUpload(HttpPostedFileBase file, string tautname, string tpapername, string tabstract, string tkeyword, string temail, string tcontact)
        {
            int count = 0;
            
            try
            {
                if (file.ContentLength > 0)
                {
                    //Database Access for Series Number
                    
                    q = "select top 1 PaperId from TBL_ARTICLEUPLOAD order by PaperId desc";
                    DataTable dt = dm.ReadBulkData(q);
                    if (dt.Rows.Count == 0)
                    {
                        count = 1;
                    }
                    else
                    {
                        count = int.Parse(dt.Rows[0][0].ToString());
                        count = count + 1;
                    }
                    //---Database Access close
                    
                    
                    
                    string filename = Path.GetFileName(file.FileName);
                    int x = filename.LastIndexOf(".");
                    string d = filename.Substring(x);
                    string cy = DateTime.Now.Year.ToString();
                    //string c = dm.Randomcode();
                    int len = tpapername.Length;
                    String pname = "";
                    for (int i = 0; i < len; i++)
                    {
                        if (tpapername[i] != ' ')
                        {
                            pname = pname + tpapername[i];
                        }
                    }
                    tpapername = pname;
                    string c = count +"_"+ tpapername;
                    //string code = cy +"_"+ c; 
                    string newfile = c+""+d;

                    string filepath = Path.Combine(Server.MapPath("~/FILEUPLOAD"), newfile);
                    file.SaveAs(filepath);
                    ViewBag.res = "File uploaded Successfully";

                    //-------------Mail Sending Procedure
                    
                    MailSender(file, tpapername, tcontact,temail,tcontact,newfile,c);
                    MailSender(file, tpapername, tcontact, "IIRJ@npgc.in", tcontact, newfile, c);

                    //--------------------End Mail Sending procedure 
                    
                    //Record Insertion In Database 
                    
                    string dtt = DateTime.Now.ToString("dd/MMM/yyyy");
                    q = "insert into TBL_ARTICLEUPLOAD values('" + tpapername + "','" + tautname + "','" + tabstract + "','" + tkeyword + "','" + temail + "','" + tcontact + "','" + dtt + "','" + c + "','"+newfile+"')";
                    bool j = dm.InsertUpdateDelete(q);
                    string datastatus = "";
                    if (j==true)
                    {
                        datastatus = "Done";
                    }
                    //---End Record Insertion Block 

                    //Confirmation Print status in table format
                    string tbl = "<table class='table table-striped'>";
                    
                    tbl += "<tr>";
                    tbl += "<th colspan='2' style='text-align:center'>Confirmation</th>";
                    tbl += "</tr>";
                    tbl += "<tr>";
                    tbl += "<td>Mail Status </td><td>Done</td>";
                    tbl += "</tr>";
                    tbl += "<tr>";
                    tbl += "<td>Database</th><td>"+datastatus+"</td>";
                    tbl += "</tr>";
                    tbl += "<tr>";
                    tbl += "<td>Reference</th><td>"+c+"";
                    tbl += "</tr>";
                    tbl += "</table>";

                    //----confirmation print status closed 



                    ViewBag.result = tbl;
                    return View();

                }
                else
                {
                    ViewBag.result = "Blank File Cannot Be Upload";
                    return View();
                }
            }
            catch
            {
                ViewBag.result = "Blank File Cannot Be Uploaded ";
            }
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult AboutCollege()
        {
            return View();
        }
        public ActionResult AboutDDUK()
        {
            return View();
        }
        public ActionResult AboutNPGCIIRJ()
        {
            return View();
        }

        public ActionResult TestingUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TestingUpload(string productN, string productP, string productQ)
        {
            return View();
        }


        public ActionResult copyrightform()
        {
            return View();
        }
        public ActionResult EditorialBoard()
        {
            return View();
        }

        public ActionResult checkmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult checkmail(string num)
        {
            string mailTitle="FirsMailFromMVC";
            string subjectmsg = "Testing Purpose";
            string bodymsg = "Testing Message for website written in body";
            string fromEmailid = "TesingWebSites@gmail.com";
            string toEmailid="anees.itlucknow@gmail.com";
            string mypass = "olmuocxglszkhdpx";

           //Mail Message 
            MailMessage message = new MailMessage(new MailAddress(fromEmailid, mailTitle), new MailAddress(toEmailid));

           //Mail Content
            message.Subject = subjectmsg;
            message.Body = bodymsg;
            message.IsBodyHtml = true;
            //Mail Attachment
            //message.Attachments.Add(new Attachment(fileToAttach.
            
            //Server details
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //Credential
            System.Net.NetworkCredential credential = new System.Net.NetworkCredential();

            credential.UserName=fromEmailid;
            credential.Password=mypass;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = credential;

            //Send Email
            smtp.Send(message);
            ViewBag.res = "Message send successfully";



            return View();
        }
        [HttpPost]
        public ActionResult Enquiry(string email,string query)
        {
            string q = "insert into TBL_ENQUIRY values('" + email + "','" + query + "')";
            bool j = dm.InsertUpdateDelete(q);
            return RedirectToAction("index", "home");
            
        }
        public ActionResult LogoutApp()
        {
            Session["uid"] = null;
            return View();
        }
        

        }//Home Controller 
    }//Name space

