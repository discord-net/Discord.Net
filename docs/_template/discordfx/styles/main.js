// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.

function toggleMenu() {
               
    var x = document.getElementById("sidebar");
    var b = document.getElementById("blackout");

    if (x.style.left === "0px") 
    {
        x.style.left = "-350px";
        b.classList.remove("showThat");
        b.classList.add("hideThat");
    } 
    else 
    {
        x.style.left = "0px";
        b.classList.remove("hideThat");
        b.classList.add("showThat");
    }
}