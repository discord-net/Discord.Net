export default
{
    iconLinks:
    [
        {
            icon: 'github',
            href: 'https://github.com/discord-net/Discord.Net',
            title: 'GitHub'
        },
        {
            icon: 'box-seam-fill',
            href: 'https://www.nuget.org/packages/Discord.Net/',
            title: 'NuGet'
        },
        {
            icon: 'discord',
            href: 'https://discord.gg/dnet',
            title: 'Discord'
        }
    ],
    start: () =>
    {
        // Ugly hack to improve toc filter.
        let target = document.getElementById("toc");
        let config = { attributes: false, childList: true, subtree: true };
        let observer = new MutationObserver((list) =>
        {
            for(const mutation of list)
            {
                if(mutation.type === "childList" && mutation.target == target)
                {
                    let filter = target.getElementsByClassName("form-control")[0];

                    let filterValue = localStorage.getItem("tocFilter");
                    let scrollValue = localStorage.getItem("tocScroll");

                    if(filterValue && filterValue !== "")
                    {
                        filter.value = filterValue;

                        let inputEvent = new Event("input");
                        filter.dispatchEvent(inputEvent);
                    }

                    // Add event to store scroll pos.
                    let tocDiv = target.getElementsByClassName("flex-fill")[0];

                    tocDiv.addEventListener("scroll", (event) =>
                    {
                        if (event.target.scrollTop >= 0)
                        {
                            localStorage.setItem("tocScroll", event.target.scrollTop);
                        }
                    });

                    if(scrollValue && scrollValue >= 0)
                    {
                        tocDiv.scroll(0, scrollValue);
                    }

                    observer.disconnect();
                    break;
                }
            }
        });

        observer.observe(target, config);
    }
}
