using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class CustomRenderMode
    {
        public static InteractiveServerRenderMode Server { get; } = new(false);
        public static InteractiveServerRenderMode ServerPrerender { get; } = new();
        public static InteractiveWebAssemblyRenderMode WebAssembly { get; } = new(false);
        public static InteractiveWebAssemblyRenderMode WebAssemblyPrerender { get; } = new();
        public static InteractiveAutoRenderMode Auto { get; } = new(false);
        public static InteractiveAutoRenderMode AutoPrerender { get; } = new();
    }
}
