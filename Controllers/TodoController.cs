using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AngularTutorial.Models;

namespace AngularTutorial.Controllers
{
    public class TodoController : ApiController
    {
        private TodoContext db = new TodoContext();

        // GET api/Todo
        //public IEnumerable<Todo> GetTodoes()
        //{
        //    return db.Todoes.AsEnumerable();
        //}

        public IEnumerable<Todo> GetTodoItems(string q = null, string sort = null, bool desc = false,
                                                        int? limit = null, int offset = 0)
        {
            var list = ((IObjectContextAdapter)db).ObjectContext.CreateObjectSet<Todo>();

            IQueryable<Todo> items = string.IsNullOrEmpty(sort) ? list.OrderBy(o => o.Priority)
                : list.OrderBy(String.Format("it.{0} {1}", sort, desc ? "DESC" : "ASC"));

            if (!string.IsNullOrEmpty(q) && q != "undefined") items = items.Where(t => t.Text.Contains(q));

            if (offset > 0) items = items.Skip(offset);
            if (limit.HasValue) items = items.Take(limit.Value);
            return items;
        }

        // GET api/Todo/5
        public Todo GetTodo(int id)
        {
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return todo;
        }

        // PUT api/Todo/5
        public HttpResponseMessage PutTodo(int id, Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != todo.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(todo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Todo
        public HttpResponseMessage PostTodo(Todo todo)
        {
            if (ModelState.IsValid)
            {
                db.Todoes.Add(todo);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todo);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todo.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Todo/5
        public HttpResponseMessage DeleteTodo(int id)
        {
            Todo todo = db.Todoes.Find(id);
            if (todo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Todoes.Remove(todo);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todo);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}