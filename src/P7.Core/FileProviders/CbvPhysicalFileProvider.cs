using System;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace P7.Core.FileProviders
{
    /// <summary>
    /// Cache Buster File Provider
    /// </summary>

    /*
     * usage: In startup.cs
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions()
        {
        FileProvider = new CbvPhysicalFileProvider(env.WebRootPath),
                RequestPath = new PathString("/cb-v")
            });


        in your razor files use this tag helper.
         <p7-script-cbv src="lib/jquery/dist/jquery.js"></pingo-script-cbv>

        this will produce something that looks like this.
         <script src="/cb-v/{version}/lib/jquery/dist/jquery.js"></script>

        which will be then be serveed by the above FileProvider discarding the {version}

        This is another way to introduce unique urls to assets for the purposes of CDN uniquness and browser cache busting.
            */
    public class CbvPhysicalFileProvider : IFileProvider, IDisposable
    {
        int NthOccurence(string s, char t, int n)
        {
            return s.TakeWhile(c => (n -= (c == t ? 1 : 0)) > 0).Count();
        }
        private PhysicalFileProvider PhysicalFileProvider { get; set; }

        public CbvPhysicalFileProvider(string root)
        {
            PhysicalFileProvider = new PhysicalFileProvider(root);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var index = NthOccurence(subpath, '/', 2);
            var newSubpath = subpath.Substring(index);
            return PhysicalFileProvider.GetFileInfo(newSubpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return PhysicalFileProvider.GetDirectoryContents(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return PhysicalFileProvider.Watch(filter);
        }

        public void Dispose()
        {
            PhysicalFileProvider.Dispose();
        }
    }
}