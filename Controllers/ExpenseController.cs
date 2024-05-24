﻿using FinanceFolio.Data;
using FinanceFolio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FinanceFolio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ExpenseController:ControllerBase
    {
        private readonly AppDbcontext _dbcontext;
        public ExpenseController(AppDbcontext dbcontext)
        {

            _dbcontext = dbcontext;

        }


        [HttpGet("GetDailyExpensesPerMonth")]
        public IActionResult GetAllExpenses(int month)
        {
            var expensePermonth = _dbcontext.Expenses.Where(e => e.ExpenseDate.Month.Equals(month))
                                                    .ToList()
                                                    .GroupBy(s => s.ExpenseDate)
                                                    .Select(x => new
                                                                {
                                                                    Date=x.Key.ToString("yyyy-MM-dd"),
                                                                    TotalAmount=x.Sum(y=>y.Amount),
                                                                }); 
                                                      
      
            return Ok(expensePermonth);
        }

        [HttpPost("InsertExpense")]
        public async Task<IActionResult> InsertExpense(ExpenseDto expense)
        {
            expense.ExpenseDate= DateTime.Now;
           
            _dbcontext.Add(expense);
            _dbcontext.SaveChanges();
            return Ok("");
        }
    }
}