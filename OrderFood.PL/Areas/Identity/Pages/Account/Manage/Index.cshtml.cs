using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using OrderFood.DAL.Entities.User;
using System.ComponentModel.DataAnnotations;

public class InputData
{
    public string? Username { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }
    [Phone]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }
    public string? Email { get; set; }

    [Display(Name = "Address")]
    public string? Address { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    public IFormFile? ProfileImage { get; set; }
    public string? uniqueFileName { get; set; }


}
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
    }

    public string? Username { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }





    // Add this property to hold the image path (for reloading)
    


        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string? Email { get; set; }

    [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? uniqueFileName { get; set; }


    // Method to handle GET request
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        Username = user.UserName;
        PhoneNumber = user.PhoneNumber;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Address = user.Address;
        DateOfBirth = user.DateOfBirth;
        Email=user.Email;
        uniqueFileName=user.Image;



        return Page();
    }

    
    // Method to handle POST request
    public async Task<IActionResult> OnPostAsync(InputData data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");


        // Upload image if provided
        if (data.ProfileImage is not null)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "users");
            Directory.CreateDirectory(uploadsFolder);

             data.uniqueFileName = Guid.NewGuid() + Path.GetExtension(data.ProfileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, data.uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await data.ProfileImage.CopyToAsync(stream);
            }

            user.Image =  data.uniqueFileName;
        }


        if (!ModelState.IsValid)
        {
            // Preserve current image if form has validation errors
            return Page();
        }

        // Update profile info
        user.FirstName = data.FirstName;
        user.LastName = data.LastName;
        user.Address =data.Address;
        user.DateOfBirth = data.DateOfBirth;
        user.PhoneNumber = data.PhoneNumber;

        //if (PhoneNumber != user.PhoneNumber)
        //{
        //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, PhoneNumber);
        //    if (!setPhoneResult.Succeeded)
        //    {
        //        StatusMessage = "Unexpected error when trying to set phone number.";
        //        return RedirectToPage();
        //    }
        //}

        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);

        StatusMessage = "Your profile has been updated";
       
        
        
        return RedirectToPage();
    
    
    
    }


}
