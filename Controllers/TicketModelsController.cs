using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TestApp1.Models;

namespace TestApp1.Controllers
{
    public class TicketModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketModels
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Department).Include(t => t.TicketStatus);
            return View(tickets.ToList());
        }

        // GET: TicketModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketModel ticketModel = db.Tickets.Find(id);
            if (ticketModel == null)
            {
                return HttpNotFound();
            }
            return View(ticketModel);
        }

        // GET: TicketModels/Create        
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentTitle");
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "StatusId", "StatusTitle");
            return View();
        }

        // POST: TicketModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId,CustomerName,Email,DepartmentId,Subject,TicketText,Time,GUIDLink,StatusId,OwnerID,Answer")] TicketModel ticketModel)
        {
            if (ModelState.IsValid)
            {
                ticketModel.Time = DateTime.Now;
                ticketModel.StatusId = 1;
                ticketModel.GUIDLink = RandomString();
                ticketModel.OwnerID = null;

                db.Tickets.Add(ticketModel);
                db.SaveChanges();


                var listTicket = db.Tickets.Where(t => t.CustomerName == ticketModel.CustomerName).ToList();
                var idTicket = listTicket.Last();

                var urlList = this.Url.Action("ShowListCustomerTicket", "TicketModels", new { name = ticketModel.CustomerName }, this.Request.Url.Scheme);
                ViewData["urlList"] = urlList;

                var urlTicket = this.Url.Action("ShowTicket", "TicketModels", new { guid = idTicket.GUIDLink }, this.Request.Url.Scheme);
                ViewData["urlTicket"] = urlList;

                SendEmail(ticketModel, urlList, urlTicket);
                return RedirectToAction("ShowTicket", "TicketModels", new { guid = idTicket.GUIDLink });
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentTitle", ticketModel.DepartmentId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "StatusId", "StatusTitle", ticketModel.StatusId);
            return View(ticketModel);
        }

        private  Random random = new Random();
        private string RandomString()
        {
             const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
             const string numb = "0123456789";
             string d1= new string (Enumerable.Repeat(chars, 3)
              .Select(s => s[random.Next(s.Length)]).ToArray());
             string d2 = new string (Enumerable.Repeat(numb, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            string uid = $"{d1}-{d2}";

            return uid;
        }

        public ActionResult ShowTicket(string guid)
        {

            //ViewData["ticketModel"] = ticketModel;

            if (guid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var listTicket = db.Tickets.Where(t => t.GUIDLink == guid).ToList();
            TicketModel ticketModel = listTicket.First();
            //TicketModel ticketModel = db.Tickets.Find(id);
            if (ticketModel == null)
            {
                return HttpNotFound();
            }
            return View(ticketModel);

        }

        public ActionResult ShowListCustomerTicket(string Name) {

           // var tickets = db.Tickets.Include(t => t.Department).Include(t => t.TicketStatus).Where(c => c.CustomerName == customer);
            
            return View(db.Tickets.Where(t => t.CustomerName == Name).ToList());
        }

        private void SendEmail(TicketModel ticket, string urlList, string urlTicket)
        {
            SmtpClient client = new SmtpClient("localhost", 25);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("samplemail@mail.com", "TestApp");
            mailMessage.To.Add(ticket.Email);

            mailMessage.Subject = ticket.CustomerName;
            //var link2 = this.Url.Action("CustomerListTicket", "Tickets", new { name = ticket.CustomerName }, this.Request.Url.Scheme);

            mailMessage.Body = "You ticket registred\n" +
                ticket.CustomerName + "\n" +
                ticket.DepartmentId + "\n" +
                ticket.Email + "\n" +
                ticket.Subject + "\n" +
                "Ticket Text: " + ticket.TicketText + "\n" +
                ticket.Time + "\n" +
                "Link to list: " + urlList + "\n" +
                "Link to ticket: " + urlTicket;

            client.Send(mailMessage);
        }

        // GET: TicketModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketModel ticketModel = db.Tickets.Find(id);
            if (ticketModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentTitle", ticketModel.DepartmentId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "StatusId", "StatusTitle", ticketModel.StatusId);
            return View(ticketModel);
        }

        // POST: TicketModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TicketId,CustomerName,Email,DepartmentId,Subject,TicketText,Time,GUIDLink,StatusId,OwnerID,Answer")] TicketModel ticketModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentTitle", ticketModel.DepartmentId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "StatusId", "StatusTitle", ticketModel.StatusId);
            return View(ticketModel);
        }

        // GET: TicketModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketModel ticketModel = db.Tickets.Find(id);
            if (ticketModel == null)
            {
                return HttpNotFound();
            }
            return View(ticketModel);
        }

        // POST: TicketModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketModel ticketModel = db.Tickets.Find(id);
            db.Tickets.Remove(ticketModel);
            db.SaveChanges();
            return RedirectToAction("Index");
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
