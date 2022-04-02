using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();
            if(id == null || id == 0) 
            { 
                //create product
                return View(company);
            }
            else
            {
                //update
                company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
                return View(company);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)
            {
                if(company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }

            return View(company);
        }

        #region API_CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companies = _unitOfWork.Company.GetAll();
            return Json(new { data = companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if(company == null)
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successfully!" });

        }

        #endregion

    }
}
