using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using ToDo.Models;
using ToDo.Models.ViewModels;

namespace ToDo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var todoListViewModel = GetAllTodos();
            return View(todoListViewModel);
        }

        internal ToDoViewModel GetAllTodos()
        {
            List<ToDoItem> todoList = new List<ToDoItem>();
            using(SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using(var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = "SELECT * FROM ToDo";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                todoList.Add(
                                    new ToDoItem
                                    {
                                        Id = reader.GetInt32(0),
                                        Name = reader.GetString(1)
                                    }
                                );
                            }
                        }
                        else
                        {
                            return new ToDoViewModel
                            {
                                ToDoList = todoList
                            };
                        }
                    }
                }
            }

            return new ToDoViewModel
            {
                ToDoList = todoList
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public RedirectResult Insert(ToDoItem todo)
        {
            using(SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using(var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Redirect("https://localhost:5001/");
        }

        public JsonResult Delete(int id)
        {
            using(SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using(var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"DELETE from ToDo WHERE Id = '{id}'";
                    tableCmd.ExecuteNonQuery();
                }
            }
            return Json(new{});
        }

        [HttpGet]
        public JsonResult PopulateForm(int id)
        {
            var todo = GetById(id);
            return Json(todo);
        }

        internal ToDoItem GetById(int id)
        {
            ToDoItem todo = new();

            using(var connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using(var tableCmd = connection.CreateCommand())
                {
                    connection.Open();
                    tableCmd.CommandText = $"SELECT * FROM ToDo WHERE Id = '{id}'";

                    using (var reader = tableCmd.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            reader.Read();
                            todo.Id = reader.GetInt32(0);
                            todo.Name = reader.GetString(1);
                        }
                        else
                        {
                            return todo;
                        }
                    }
                }
            }

            return todo;
        }

        
        public RedirectResult Update(ToDoItem todo)
        {
            using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var tableCmd = con.CreateCommand())
                {
                    con.Open();
                    tableCmd.CommandText = $"UPDATE ToDo SET (Name) = '{todo.Name}' WHERE Id = '{todo.Id}'";
                    try
                    {
                        tableCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return Redirect("https://localhost:5001/");
        }
    }
}
