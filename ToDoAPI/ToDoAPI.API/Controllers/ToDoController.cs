using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins: "*", headers:"*", methods:"*")]
    public class ToDoController : ApiController
    {
        //Create a connection to the db
        ToDoEntities db = new ToDoEntities();

        //GET - api/Categories
        public IHttpActionResult GetToDo()
        {
            List<ToDoViewModel> toDo = db.ToDoItems.Include("Category").Select(t => new ToDoViewModel()
            {
                ToDoId = t.ToDoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).ToList<ToDoViewModel>();
            if (toDo.Count == 0)
            {
                return NotFound();
            }
            return Ok(toDo);
        }//end GetToDoItems

        //GET = api/todo/id
        public IHttpActionResult GetToDo(int id)
        {
            ToDoViewModel toDo = db.ToDoItems.Include("Category").Where(t => t.ToDoId == id).Select(t => new ToDoViewModel()
            {
                ToDoId = t.ToDoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId,
                Category = new CategoryViewModel()
                {
                    CategoryId = t.Category.CategoryId,
                    Name = t.Category.Name,
                    Description = t.Category.Description
                }
            }).FirstOrDefault();

            if (toDo == null)
            {
                return NotFound();
            }

            return Ok(toDo);
        }//end Get ToDo

        //POST - api/ToDo (HTTPPost)
        public IHttpActionResult PostToDo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            ToDoItem newToDo = new ToDoItem()
            {
                Action = toDo.Action,
                Done = toDo.Done,
                CategoryId = toDo.CategoryId
            };

            db.ToDoItems.Add(newToDo);
            db.SaveChanges();
            return Ok(newToDo);
        }//end PostToDo

        //PUT - api/toDo (HTTPPut)
        public IHttpActionResult PutToDo(ToDoViewModel toDo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            ToDoItem existingToDo = db.ToDoItems.Where(t => t.ToDoId == toDo.ToDoId).FirstOrDefault();

            if (existingToDo != null)
            {
                existingToDo.ToDoId = toDo.ToDoId;
                existingToDo.Action = toDo.Action;
                existingToDo.CategoryId = toDo.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end Put

        //DELETE -api/ToDo/if (HTTPDelete)
        public IHttpActionResult DeleteToDo(int id)
        {
            ToDoItem toDo = db.ToDoItems.Where(t => t.ToDoId == id).FirstOrDefault();
            if (toDo != null)
            {
                db.ToDoItems.Remove(toDo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end Delete

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }//end class
}//end namespace
