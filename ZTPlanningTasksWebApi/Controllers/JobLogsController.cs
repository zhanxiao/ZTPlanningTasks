using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ZTPlanningTasksWebApi.Models;

namespace ZTPlanningTasksWebApi.Controllers
{
    public class JobLogsController : ApiController
    {
        private ZTPlanningTasksEntities db = new ZTPlanningTasksEntities();

        // GET: api/JobLogs
        public ResultData GetT_JobLog(int page, int limit, string taskName)
        {
            var jobLogList = db.T_JobLog.Select(m => m);
            if(!string.IsNullOrEmpty(taskName))
            {
                jobLogList = jobLogList.Where(m => m.JobName == taskName);
            }
            var result = jobLogList.OrderByDescending(m => m.CreateTime).Skip((page - 1) * limit).Take(limit);
            return new ResultData {
                Data = result,
                Total = jobLogList.Count()
            };
        }

        // GET: api/JobLogs/5
        [ResponseType(typeof(T_JobLog))]
        public IHttpActionResult GetT_JobLog(int id)
        {
            T_JobLog T_JobLog = db.T_JobLog.Find(id);
            if (T_JobLog == null)
            {
                return NotFound();
            }

            return Ok(T_JobLog);
        }

        // PUT: api/JobLogs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutT_JobLog(int id, T_JobLog T_JobLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != T_JobLog.Id)
            {
                return BadRequest();
            }

            db.Entry(T_JobLog).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T_JobLogExists(id))
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

        // POST: api/JobLogs
        [ResponseType(typeof(T_JobLog))]
        public IHttpActionResult PostT_JobLog(T_JobLog T_JobLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.T_JobLog.Add(T_JobLog);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = T_JobLog.Id }, T_JobLog);
        }

        // DELETE: api/JobLogs/5
        [ResponseType(typeof(T_JobLog))]
        public IHttpActionResult DeleteT_JobLog(int id)
        {
            T_JobLog T_JobLog = db.T_JobLog.Find(id);
            if (T_JobLog == null)
            {
                return NotFound();
            }

            db.T_JobLog.Remove(T_JobLog);
            db.SaveChanges();

            return Ok(T_JobLog);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool T_JobLogExists(int id)
        {
            return db.T_JobLog.Count(e => e.Id == id) > 0;
        }
    }
}