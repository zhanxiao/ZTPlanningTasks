using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ZTPlanningTasksWebApi.Models;

namespace ZTPlanningTasksWebApi.Controllers
{
    public class TasksController : ApiController
    {
        private ZTPlanningTasksEntities db = new ZTPlanningTasksEntities();

        // GET: api/T_Tasks
        public ResultData GetT_Tasks(int page, int limit, string taskName)
        {
            var taskList = db.T_Tasks.Select(m => m);
            if (!string.IsNullOrEmpty(taskName))
            {
                taskList = taskList.Where(m => m.JobName.Contains(taskName));
            }
            var result = taskList.OrderBy(m => m.CreateTime).Skip((page - 1) * limit).Take(limit);
            return new ResultData
            {
                Data = result,
                Total = taskList.Count()
            };
        }

        // GET: api/T_Tasks/5
        [ResponseType(typeof(T_Tasks))]
        public IHttpActionResult GetT_Tasks(int id)
        {
            T_Tasks t_Tasks = db.T_Tasks.Find(id);
            if (t_Tasks == null)
            {
                return NotFound();
            }

            return Ok(t_Tasks);
        }

        // PUT: api/T_Tasks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutT_Tasks(int id, T_Tasks t_Tasks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != t_Tasks.Id)
            {
                return BadRequest();
            }

            db.Entry(t_Tasks).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T_TasksExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/T_Tasks
        [ResponseType(typeof(T_Tasks))]
        public IHttpActionResult PostT_Tasks(T_Tasks t_Tasks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.T_Tasks.Add(t_Tasks);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = t_Tasks.Id }, t_Tasks);
        }

        // DELETE: api/T_Tasks/5
        [ResponseType(typeof(T_Tasks))]
        public IHttpActionResult DeleteT_Tasks(int id)
        {
            T_Tasks t_Tasks = db.T_Tasks.Find(id);
            if (t_Tasks == null)
            {
                return NotFound();
            }

            db.T_Tasks.Remove(t_Tasks);
            db.SaveChanges();

            return Ok(t_Tasks);
        }

        public IHttpActionResult ChangeState(string command)
        {
            try
            {
                using (NamedPipeClientStream client = new NamedPipeClientStream(".", "ZTPlanningTasksCore", PipeDirection.InOut))
                {
                    client.Connect();
                    using (StreamWriter sw = new StreamWriter(client))
                    {
                        sw.WriteLine(command);
                    }
                }
                return Ok("success");
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool T_TasksExists(int id)
        {
            return db.T_Tasks.Count(e => e.Id == id) > 0;
        }
    }
}