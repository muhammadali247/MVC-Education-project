﻿using Microsoft.AspNetCore.Mvc;

namespace EduHome.ViewComponents;

public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
