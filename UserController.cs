using FormCreation.IService;
using FormCreation.Models;
using FormCreation.Repository;
using FormCreation.Service;
using System.Linq;
using System.Web.Mvc;

public class UserController : Controller
{
    private readonly IUserService _svc;

    public UserController()
    {
        _svc = new UserService(new UserRepository());
    }

    public ActionResult Index()
    {
        return View();
    }

    public JsonResult GetCountries()
    {
        return Json(_svc.GetCountries(), JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetStates(int countryId)
    {
        return Json(_svc.GetStatesByCountry(countryId), JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetDistricts(int stateId)
    {
        return Json(_svc.GetDistrictsByState(stateId), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult SaveUser(UserDetails model)
    {
        _svc.SaveUser(model);
        return Json(new { success = true, message = "Saved successfully" });
    }

    public JsonResult GetAllUsers()
    {
        return Json(_svc.GetUsers(), JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetUserById(int id)
    {
        return Json(_svc.GetUserById(id), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult UpdateUser(UserDetails u)
    {
        _svc.UpdateUser(u);
        return Json(new { success = true, message = "Updated successfully" });
    }

    [HttpPost]
    public JsonResult DeleteUser(int id)
    {
        _svc.DeleteUser(id);
        return Json(new { success = true, message = "Deleted successfully" });
    }
    public JsonResult GetAuditLogs()
    {
        var data = _svc.GetAuditLogs();

        var result = data.Select(a => new
        {
            a.ActionType,
            a.OldData,
            a.NewData,
            DoneDate = a.DoneDate.ToString("yyyy-MM-dd HH:mm:ss") // ✅ FIX
        });

        return Json(result, JsonRequestBehavior.AllowGet);
    }
}