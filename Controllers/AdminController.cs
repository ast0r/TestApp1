using Microsoft.AspNet.Identity.EntityFramework;
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
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult NewUnassignedTickets()
        {
            var newUnassignedTickets = db.Tickets.Where(t => t.StatusId == 1);
            return View(newUnassignedTickets.ToList());
        }

        public ActionResult OpenTickets()
        {
            var openTickets = db.Tickets.Where(t => t.StatusId == 2 );
            return View(openTickets.ToList());
        }

        public ActionResult OnHoldTickets()
        {
            var onHoldTickets = db.Tickets.Where(t => t.StatusId == 3);
            return View(onHoldTickets.ToList());
        }

        public ActionResult ClosedTickets()
        {
            var closedTickets = db.Tickets.Where(t => t.StatusId == 4);
            var tmp = db.Tickets.Where(t=> t.StatusId == 5);

            closedTickets = closedTickets.Concat(tmp);

            return View(closedTickets.ToList());
        }

        // GET: Admin
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Department).Include(t => t.TicketStatus);
            return View(tickets.ToList());
        }

        // GET: Admin/Details/5
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

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId,CustomerName,Email,DepartmentId,Subject,TicketText,Time,GUIDLink,StatusId,OwnerID,Answer")] TicketModel ticketModel)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticketModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentTitle", ticketModel.DepartmentId);
            ViewBag.StatusId = new SelectList(db.TicketStatuses, "StatusId", "StatusTitle", ticketModel.StatusId);
            return View(ticketModel);
        }

        // GET: Admin/Edit/5
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

        // POST: Admin/Edit/5
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

           // var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
           // ViewBag.OwnerID = new SelectList(userManager.Users, "Id", "UserName", ticketModel.OwnerID);

            return View(ticketModel);
        }

        // GET: Admin/Delete/5
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

        // POST: Admin/Delete/5
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
    }
}
