﻿namespace Chains.Play.Web.HttpListener.Handlers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml;
    using Chains;
    using Chains.Play.Web;

    public sealed class FileSystemHttpRequestHandler : Chain<FileSystemHttpRequestHandler>, IHttpRequestHandler
    {
        private const string ContentTypes = @"<?xml version=""1.0""?>
<mimeTypes>
  <mimeType>
    <fileType>ai</fileType>
    <mime>application/postscript</mime>
  </mimeType>
  <mimeType>
    <fileType>aif</fileType>
    <mime>audio/x-aiff</mime>
  </mimeType>
  <mimeType>
    <fileType>aifc</fileType>
    <mime>audio/x-aiff</mime>
  </mimeType>
  <mimeType>
    <fileType>aiff</fileType>
    <mime>audio/x-aiff</mime>
  </mimeType>
  <mimeType>
    <fileType>asc</fileType>
    <mime>text/plain</mime>
  </mimeType>
  <mimeType>
    <fileType>atom</fileType>
    <mime>application/atom+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>au</fileType>
    <mime>audio/basic</mime>
  </mimeType>
  <mimeType>
    <fileType>avi</fileType>
    <mime>video/x-msvideo</mime>
  </mimeType>
  <mimeType>
    <fileType>bcpio</fileType>
    <mime>application/x-bcpio</mime>
  </mimeType>
  <mimeType>
    <fileType>bin</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>bmp</fileType>
    <mime>image/bmp</mime>
  </mimeType>
  <mimeType>
    <fileType>cdf</fileType>
    <mime>application/x-netcdf</mime>
  </mimeType>
  <mimeType>
    <fileType>cgm</fileType>
    <mime>image/cgm</mime>
  </mimeType>
  <mimeType>
    <fileType>class</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>cpio</fileType>
    <mime>application/x-cpio</mime>
  </mimeType>
  <mimeType>
    <fileType>cpt</fileType>
    <mime>application/mac-compactpro</mime>
  </mimeType>
  <mimeType>
    <fileType>csh</fileType>
    <mime>application/x-csh</mime>
  </mimeType>
  <mimeType>
    <fileType>css</fileType>
    <mime>text/css</mime>
  </mimeType>
  <mimeType>
    <fileType>dcr</fileType>
    <mime>application/x-director</mime>
  </mimeType>
  <mimeType>
    <fileType>dif</fileType>
    <mime>video/x-dv</mime>
  </mimeType>
  <mimeType>
    <fileType>dir</fileType>
    <mime>application/x-director</mime>
  </mimeType>
  <mimeType>
    <fileType>djv</fileType>
    <mime>image/vnd.djvu</mime>
  </mimeType>
  <mimeType>
    <fileType>djvu</fileType>
    <mime>image/vnd.djvu</mime>
  </mimeType>
  <mimeType>
    <fileType>dll</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>dmg</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>dms</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>doc</fileType>
    <mime>application/msword</mime>
  </mimeType>
  <mimeType>
    <fileType>dtd</fileType>
    <mime>application/xml-dtd</mime>
  </mimeType>
  <mimeType>
    <fileType>dv</fileType>
    <mime>video/x-dv</mime>
  </mimeType>
  <mimeType>
    <fileType>dvi</fileType>
    <mime>application/x-dvi</mime>
  </mimeType>
  <mimeType>
    <fileType>dxr</fileType>
    <mime>application/x-director</mime>
  </mimeType>
  <mimeType>
    <fileType>eps</fileType>
    <mime>application/postscript</mime>
  </mimeType>
  <mimeType>
    <fileType>etx</fileType>
    <mime>text/x-setext</mime>
  </mimeType>
  <mimeType>
    <fileType>exe</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>ez</fileType>
    <mime>application/andrew-inset</mime>
  </mimeType>
  <mimeType>
    <fileType>gif</fileType>
    <mime>image/gif</mime>
  </mimeType>
  <mimeType>
    <fileType>gram</fileType>
    <mime>application/srgs</mime>
  </mimeType>
  <mimeType>
    <fileType>grxml</fileType>
    <mime>application/srgs+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>gtar</fileType>
    <mime>application/x-gtar</mime>
  </mimeType>
  <mimeType>
    <fileType>hdf</fileType>
    <mime>application/x-hdf</mime>
  </mimeType>
  <mimeType>
    <fileType>hqx</fileType>
    <mime>application/mac-binhex40</mime>
  </mimeType>
  <mimeType>
    <fileType>htm</fileType>
    <mime>text/html</mime>
  </mimeType>
  <mimeType>
    <fileType>html</fileType>
    <mime>text/html</mime>
  </mimeType>
  <mimeType>
    <fileType>ice</fileType>
    <mime>x-conference/x-cooltalk</mime>
  </mimeType>
  <mimeType>
    <fileType>ico</fileType>
    <mime>image/x-icon</mime>
  </mimeType>
  <mimeType>
    <fileType>ics</fileType>
    <mime>text/calendar</mime>
  </mimeType>
  <mimeType>
    <fileType>ief</fileType>
    <mime>image/ief</mime>
  </mimeType>
  <mimeType>
    <fileType>ifb</fileType>
    <mime>text/calendar</mime>
  </mimeType>
  <mimeType>
    <fileType>iges</fileType>
    <mime>model/iges</mime>
  </mimeType>
  <mimeType>
    <fileType>igs</fileType>
    <mime>model/iges</mime>
  </mimeType>
  <mimeType>
    <fileType>jnlp</fileType>
    <mime>application/x-java-jnlp-file</mime>
  </mimeType>
  <mimeType>
    <fileType>jp2</fileType>
    <mime>image/jp2</mime>
  </mimeType>
  <mimeType>
    <fileType>jpe</fileType>
    <mime>image/jpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>jpeg</fileType>
    <mime>image/jpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>jpg</fileType>
    <mime>image/jpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>js</fileType>
    <mime>application/x-javascript</mime>
  </mimeType>
  <mimeType>
    <fileType>kar</fileType>
    <mime>audio/midi</mime>
  </mimeType>
  <mimeType>
    <fileType>latex</fileType>
    <mime>application/x-latex</mime>
  </mimeType>
  <mimeType>
    <fileType>lha</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>lzh</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>m3u</fileType>
    <mime>audio/x-mpegurl</mime>
  </mimeType>
  <mimeType>
    <fileType>m4a</fileType>
    <mime>audio/mp4a-latm</mime>
  </mimeType>
  <mimeType>
    <fileType>m4b</fileType>
    <mime>audio/mp4a-latm</mime>
  </mimeType>
  <mimeType>
    <fileType>m4p</fileType>
    <mime>audio/mp4a-latm</mime>
  </mimeType>
  <mimeType>
    <fileType>m4u</fileType>
    <mime>video/vnd.mpegurl</mime>
  </mimeType>
  <mimeType>
    <fileType>m4v</fileType>
    <mime>video/x-m4v</mime>
  </mimeType>
  <mimeType>
    <fileType>mac</fileType>
    <mime>image/x-macpaint</mime>
  </mimeType>
  <mimeType>
    <fileType>man</fileType>
    <mime>application/x-troff-man</mime>
  </mimeType>
  <mimeType>
    <fileType>mathml</fileType>
    <mime>application/mathml+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>me</fileType>
    <mime>application/x-troff-me</mime>
  </mimeType>
  <mimeType>
    <fileType>mesh</fileType>
    <mime>model/mesh</mime>
  </mimeType>
  <mimeType>
    <fileType>mid</fileType>
    <mime>audio/midi</mime>
  </mimeType>
  <mimeType>
    <fileType>midi</fileType>
    <mime>audio/midi</mime>
  </mimeType>
  <mimeType>
    <fileType>mif</fileType>
    <mime>application/vnd.mif</mime>
  </mimeType>
  <mimeType>
    <fileType>mov</fileType>
    <mime>video/quicktime</mime>
  </mimeType>
  <mimeType>
    <fileType>movie</fileType>
    <mime>video/x-sgi-movie</mime>
  </mimeType>
  <mimeType>
    <fileType>mp2</fileType>
    <mime>audio/mpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>mp3</fileType>
    <mime>audio/mpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>mp4</fileType>
    <mime>video/mp4</mime>
  </mimeType>
  <mimeType>
    <fileType>mpe</fileType>
    <mime>video/mpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>mpeg</fileType>
    <mime>video/mpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>mpg</fileType>
    <mime>video/mpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>mpga</fileType>
    <mime>audio/mpeg</mime>
  </mimeType>
  <mimeType>
    <fileType>ms</fileType>
    <mime>application/x-troff-ms</mime>
  </mimeType>
  <mimeType>
    <fileType>msh</fileType>
    <mime>model/mesh</mime>
  </mimeType>
  <mimeType>
    <fileType>mxu</fileType>
    <mime>video/vnd.mpegurl</mime>
  </mimeType>
  <mimeType>
    <fileType>nc</fileType>
    <mime>application/x-netcdf</mime>
  </mimeType>
  <mimeType>
    <fileType>oda</fileType>
    <mime>application/oda</mime>
  </mimeType>
  <mimeType>
    <fileType>ogg</fileType>
    <mime>application/ogg</mime>
  </mimeType>
  <mimeType>
    <fileType>pbm</fileType>
    <mime>image/x-portable-bitmap</mime>
  </mimeType>
  <mimeType>
    <fileType>pct</fileType>
    <mime>image/pict</mime>
  </mimeType>
  <mimeType>
    <fileType>pdb</fileType>
    <mime>chemical/x-pdb</mime>
  </mimeType>
  <mimeType>
    <fileType>pdf</fileType>
    <mime>application/pdf</mime>
  </mimeType>
  <mimeType>
    <fileType>pgm</fileType>
    <mime>image/x-portable-graymap</mime>
  </mimeType>
  <mimeType>
    <fileType>pgn</fileType>
    <mime>application/x-chess-pgn</mime>
  </mimeType>
  <mimeType>
    <fileType>pic</fileType>
    <mime>image/pict</mime>
  </mimeType>
  <mimeType>
    <fileType>pict</fileType>
    <mime>image/pict</mime>
  </mimeType>
  <mimeType>
    <fileType>png</fileType>
    <mime>image/png</mime>
  </mimeType>
  <mimeType>
    <fileType>pnm</fileType>
    <mime>image/x-portable-anymap</mime>
  </mimeType>
  <mimeType>
    <fileType>pnt</fileType>
    <mime>image/x-macpaint</mime>
  </mimeType>
  <mimeType>
    <fileType>pntg</fileType>
    <mime>image/x-macpaint</mime>
  </mimeType>
  <mimeType>
    <fileType>ppm</fileType>
    <mime>image/x-portable-pixmap</mime>
  </mimeType>
  <mimeType>
    <fileType>ppt</fileType>
    <mime>application/vnd.ms-powerpoint</mime>
  </mimeType>
  <mimeType>
    <fileType>ps</fileType>
    <mime>application/postscript</mime>
  </mimeType>
  <mimeType>
    <fileType>qt</fileType>
    <mime>video/quicktime</mime>
  </mimeType>
  <mimeType>
    <fileType>qti</fileType>
    <mime>image/x-quicktime</mime>
  </mimeType>
  <mimeType>
    <fileType>qtif</fileType>
    <mime>image/x-quicktime</mime>
  </mimeType>
  <mimeType>
    <fileType>ra</fileType>
    <mime>audio/x-pn-realaudio</mime>
  </mimeType>
  <mimeType>
    <fileType>ram</fileType>
    <mime>audio/x-pn-realaudio</mime>
  </mimeType>
  <mimeType>
    <fileType>ras</fileType>
    <mime>image/x-cmu-raster</mime>
  </mimeType>
  <mimeType>
    <fileType>rdf</fileType>
    <mime>application/rdf+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>rgb</fileType>
    <mime>image/x-rgb</mime>
  </mimeType>
  <mimeType>
    <fileType>rm</fileType>
    <mime>application/vnd.rn-realmedia</mime>
  </mimeType>
  <mimeType>
    <fileType>roff</fileType>
    <mime>application/x-troff</mime>
  </mimeType>
  <mimeType>
    <fileType>rtf</fileType>
    <mime>text/rtf</mime>
  </mimeType>
  <mimeType>
    <fileType>rtx</fileType>
    <mime>text/richtext</mime>
  </mimeType>
  <mimeType>
    <fileType>sgm</fileType>
    <mime>text/sgml</mime>
  </mimeType>
  <mimeType>
    <fileType>sgml</fileType>
    <mime>text/sgml</mime>
  </mimeType>
  <mimeType>
    <fileType>sh</fileType>
    <mime>application/x-sh</mime>
  </mimeType>
  <mimeType>
    <fileType>shar</fileType>
    <mime>application/x-shar</mime>
  </mimeType>
  <mimeType>
    <fileType>silo</fileType>
    <mime>model/mesh</mime>
  </mimeType>
  <mimeType>
    <fileType>sit</fileType>
    <mime>application/x-stuffit</mime>
  </mimeType>
  <mimeType>
    <fileType>skd</fileType>
    <mime>application/x-koan</mime>
  </mimeType>
  <mimeType>
    <fileType>skm</fileType>
    <mime>application/x-koan</mime>
  </mimeType>
  <mimeType>
    <fileType>skp</fileType>
    <mime>application/x-koan</mime>
  </mimeType>
  <mimeType>
    <fileType>skt</fileType>
    <mime>application/x-koan</mime>
  </mimeType>
  <mimeType>
    <fileType>smi</fileType>
    <mime>application/smil</mime>
  </mimeType>
  <mimeType>
    <fileType>smil</fileType>
    <mime>application/smil</mime>
  </mimeType>
  <mimeType>
    <fileType>snd</fileType>
    <mime>audio/basic</mime>
  </mimeType>
  <mimeType>
    <fileType>so</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>spl</fileType>
    <mime>application/x-futuresplash</mime>
  </mimeType>
  <mimeType>
    <fileType>src</fileType>
    <mime>application/x-wais-source</mime>
  </mimeType>
  <mimeType>
    <fileType>sv4cpio</fileType>
    <mime>application/x-sv4cpio</mime>
  </mimeType>
  <mimeType>
    <fileType>sv4crc</fileType>
    <mime>application/x-sv4crc</mime>
  </mimeType>
  <mimeType>
    <fileType>svg</fileType>
    <mime>image/svg+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>swf</fileType>
    <mime>application/x-shockwave-flash</mime>
  </mimeType>
  <mimeType>
    <fileType>t</fileType>
    <mime>application/x-troff</mime>
  </mimeType>
  <mimeType>
    <fileType>tar</fileType>
    <mime>application/x-tar</mime>
  </mimeType>
  <mimeType>
    <fileType>tcl</fileType>
    <mime>application/x-tcl</mime>
  </mimeType>
  <mimeType>
    <fileType>tex</fileType>
    <mime>application/x-tex</mime>
  </mimeType>
  <mimeType>
    <fileType>texi</fileType>
    <mime>application/x-texinfo</mime>
  </mimeType>
  <mimeType>
    <fileType>texinfo</fileType>
    <mime>application/x-texinfo</mime>
  </mimeType>
  <mimeType>
    <fileType>tif</fileType>
    <mime>image/tiff</mime>
  </mimeType>
  <mimeType>
    <fileType>tiff</fileType>
    <mime>image/tiff</mime>
  </mimeType>
  <mimeType>
    <fileType>tr</fileType>
    <mime>application/x-troff</mime>
  </mimeType>
  <mimeType>
    <fileType>tsv</fileType>
    <mime>text/tab-separated-values</mime>
  </mimeType>
  <mimeType>
    <fileType>txt</fileType>
    <mime>text/plain</mime>
  </mimeType>
  <mimeType>
    <fileType>ustar</fileType>
    <mime>application/x-ustar</mime>
  </mimeType>
  <mimeType>
    <fileType>vcd</fileType>
    <mime>application/x-cdlink</mime>
  </mimeType>
  <mimeType>
    <fileType>vrml</fileType>
    <mime>model/vrml</mime>
  </mimeType>
  <mimeType>
    <fileType>vxml</fileType>
    <mime>application/voicexml+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>wav</fileType>
    <mime>audio/x-wav</mime>
  </mimeType>
  <mimeType>
    <fileType>wbmp</fileType>
    <mime>image/vnd.wap.wbmp</mime>
  </mimeType>
  <mimeType>
    <fileType>wbmxl</fileType>
    <mime>application/vnd.wap.wbxml</mime>
  </mimeType>
  <mimeType>
    <fileType>wml</fileType>
    <mime>text/vnd.wap.wml</mime>
  </mimeType>
  <mimeType>
    <fileType>wmlc</fileType>
    <mime>application/vnd.wap.wmlc</mime>
  </mimeType>
  <mimeType>
    <fileType>wmls</fileType>
    <mime>text/vnd.wap.wmlscript</mime>
  </mimeType>
  <mimeType>
    <fileType>wmlsc</fileType>
    <mime>application/vnd.wap.wmlscriptc</mime>
  </mimeType>
  <mimeType>
    <fileType>wrl</fileType>
    <mime>model/vrml</mime>
  </mimeType>
  <mimeType>
    <fileType>xbm</fileType>
    <mime>image/x-xbitmap</mime>
  </mimeType>
  <mimeType>
    <fileType>xht</fileType>
    <mime>application/xhtml+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>xhtml</fileType>
    <mime>application/xhtml+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>xls</fileType>
    <mime>application/vnd.ms-excel</mime>
  </mimeType>
  <mimeType>
    <fileType>xml</fileType>
    <mime>application/xml</mime>
  </mimeType>
  <mimeType>
    <fileType>xpm</fileType>
    <mime>image/x-xpixmap</mime>
  </mimeType>
  <mimeType>
    <fileType>xsl</fileType>
    <mime>application/xml</mime>
  </mimeType>
  <mimeType>
    <fileType>xslt</fileType>
    <mime>application/xslt+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>xul</fileType>
    <mime>application/vnd.mozilla.xul+xml</mime>
  </mimeType>
  <mimeType>
    <fileType>xwd</fileType>
    <mime>image/x-xwindowdump</mime>
  </mimeType>
  <mimeType>
    <fileType>xyz</fileType>
    <mime>chemical/x-xyz</mime>
  </mimeType>
  <mimeType>
    <fileType>zip</fileType>
    <mime>application/zip</mime>
  </mimeType>
  <mimeType>
    <fileType>woff</fileType>
    <mime>application/x-font-woff</mime>
  </mimeType>
  <mimeType>
    <fileType>ttf</fileType>
    <mime>application/octet-stream</mime>
  </mimeType>
  <mimeType>
    <fileType>eot</fileType>
    <mime>application/vnd.ms-fontobject</mime>
  </mimeType>
</mimeTypes>";

        private readonly XmlDocument contentTypes;

        private readonly string absoluteBaseFolder;

        public FileSystemHttpRequestHandler(
            HttpServer httpServer,
            string absoluteBaseFolder = null,
            string[] fileSearches = null,
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            absoluteBaseFolder = absoluteBaseFolder ?? AppDomain.CurrentDomain.BaseDirectory;
            this.absoluteBaseFolder = absoluteBaseFolder;

            contentTypes = new XmlDocument();
            contentTypes.LoadXml(ContentTypes);

            if (fileSearches == null)
            {
                var extensionsNodes = contentTypes.SelectNodes("/mimeTypes/mimeType/fileType/text()");
                fileSearches = new string[extensionsNodes.Count];
                for (var i = 0; i < fileSearches.Length; ++i)
                {
                    fileSearches[i] = "*." + extensionsNodes[i].Value;
                }
            }

            foreach (var searchPattern in fileSearches)
            {
                var filesToRegister = Directory.GetFiles(absoluteBaseFolder, searchPattern, searchOption);

                foreach (var fileToRegister in filesToRegister)
                {
                    if (fileToRegister.EndsWith(searchPattern.Substring(1)))
                    {
                        var relativeFile = fileToRegister.Substring(absoluteBaseFolder.Length);
                        var virtualFile = "/" + relativeFile.Replace(Path.DirectorySeparatorChar, '/') + "/";
                        httpServer.AddPath(virtualFile);
                    }
                }
            }

            httpServer.Modules.Add(this);
        }

        public bool ResolveRequest(HttpListenerContext context)
        {
            var path = absoluteBaseFolder
                + context.Request.Url.LocalPath.Substring(1).Replace('/', Path.DirectorySeparatorChar);

            if (File.Exists(path))
            {
                var extension = Path.GetExtension(path).ToLowerInvariant();

                var node =
                    contentTypes.SelectSingleNode(
                        "/mimeTypes/mimeType[fileType/text()='" + extension.Substring(1) + "']/mime/text()");

                if (node != null)
                {
                    new HttpResultContext(contentType: node.Value).SendFile(path)
                                                                  .AccessControlAllowOriginAll()
                                                                  .ApplyOutputToHttpContext(context);

                    return true;
                }
            }

            return false;
        }
    }
}