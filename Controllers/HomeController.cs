using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImoSphere.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ImoSphere.Models;
using System.Collections.Generic;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

private static List<Message> _messages = new List<Message>();
    private static int _messageIdCounter = 1;
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }

    public async Task<IActionResult> Properties()
    {
        var properties = await _context.Properties.ToListAsync();
        return View(properties); 
    }

    public IActionResult Services()
    {
        return View();
    }

    public IActionResult ContactUs()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
     [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitContactForm(string Name, string Email, string Message)
    {
        _messages.Add(new Message
        {
            Id = _messageIdCounter++, 
            Name = Name,
            Email = Email,
            Content = Message,
            IsRead = false
        });

        TempData["SuccessMessage"] = "Your message has been sent successfully!";
        return RedirectToAction("ContactUs");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult ViewMessages()
    {
        return View(_messages);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult MarkAsRead(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            message.IsRead = true;
        }

        return RedirectToAction("ViewMessages");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult DeleteMessage(int id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
        {
            _messages.Remove(message);
        }

        return RedirectToAction("ViewMessages");
    }
    [Authorize(Roles = "Admin")]
    public IActionResult AdminUser()
    {
        return View();
    }
}
