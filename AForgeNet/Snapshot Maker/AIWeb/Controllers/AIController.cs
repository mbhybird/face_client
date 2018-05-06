using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIWeb.Controllers
{
    public class AIController : Controller
    {
        aidbEntities ctx = new aidbEntities();
        // GET: AI
        public JsonResult GetQueue()
        {
            var queue = ctx.queuetb.FirstOrDefault();
            if (queue != null)
            {
                return Json(new Result<GetQueueResult>()
                {
                    status = 1,
                    result = new GetQueueResult()
                    {

                        age = (int)queue.age,
                        gender = queue.gender,
                        glasses = (int)queue.glasses,
                        groupid = queue.groupid,
                        race = queue.race,
                        uid = queue.uid,
                        type = queue.type,
                        userinfo = queue.userinfo
                    }

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Result<ErrorResult>()
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SaveUserData(string id, string name) {
            var fr = ctx.frtb.SingleOrDefault(x => x.uid == id);
            if (fr != null)
            {
                fr.username = name;
                if (ctx.SaveChanges() > 0)
                {
                    return Json(new Result<OkResult>()
                    {
                        status = 1,
                        result = new OkResult()
                        {
                            ok_msg = "user name saved"
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new Result<ErrorResult>()
                    {
                        status = 0,
                        result = new ErrorResult()
                        {
                            error_code = 1001
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new Result<ErrorResult>()
                {
                    status = 0,
                    result = new ErrorResult()
                    {
                        error_code = 1002
                    }
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetUserData(string id)
        {
            var fr = ctx.frtb.SingleOrDefault(x => x.uid == id);

            if (fr != null)
            {
                return Json(new Result<GetUserDataResult>()
                {
                    status = 1,
                    result = new GetUserDataResult()
                    {
                        age = fr.age,
                        gender = fr.gender,
                        glasses = fr.glasses,
                        groupid = fr.groupid,
                        race = fr.race,
                        uid = fr.uid,
                        userinfo = fr.userinfo,
                        username = fr.username
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new Result<ErrorResult>()
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteQueue()
        {
            var queue = ctx.queuetb.FirstOrDefault();
            if (queue != null)
            {
                var isOldUser = ctx.frtb.Any(x => x.uid == queue.uid);
                if (isOldUser)
                {
                    //do nothing
                }
                else
                {
                    ctx.frtb.Add(new frtb()
                    {
                        age = queue.age,
                        gender = queue.gender,
                        back = queue.back,
                        front = queue.front,
                        left = queue.left,
                        right = queue.right,
                        glasses = queue.glasses,
                        groupid = queue.groupid,
                        race = queue.race,
                        uid = queue.uid,
                        userinfo = queue.userinfo

                    });
                }
            }

            foreach (var q in ctx.queuetb)
            {
                ctx.queuetb.Remove(q);
            }

            if (ctx.SaveChanges() > 0)
            {
                return Json(new Result<OkResult>()
                {
                    status = 1
                }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new Result<ErrorResult>()
                {
                    status = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}