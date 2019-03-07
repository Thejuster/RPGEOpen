﻿using RpgeOpen.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RpgeOpen.IDE.Utils
{
    internal static class TiledImporter
    {
        public static void ImportTmx( string tmxPath, string projectDir ) {
            if(!Path.GetExtension(tmxPath).Contains( "tmx" ))
                throw new ArgumentException("Invalid file");

            var tmxDir = Path.GetDirectoryName(tmxPath);

            using( var tmxReadStream = File.OpenRead( tmxPath ) ) {
                var document = XDocument.Load(tmxReadStream);
                var tilesets = document.Root.Elements().Where( e => e.Name == "tileset");
                foreach(var ts in tilesets ) {
                    var source = ts.Attributes().First(a => a.Name == "source");
                    ImportTileSheet( Path.Combine(tmxDir,source.Value), projectDir );
                    source.Value = Path.Combine( "..", Project.Paths.TileSheets, Path.GetFileName(source.Value) );
                }

                document.Save(Path.Combine( projectDir, Project.Paths.Maps, Path.GetFileName(tmxPath) ));
            }
        }

        private static void ImportTileSheet( string tsxPath, string projectDir ) {
            var tsxDir = Path.GetDirectoryName(tsxPath);

            using( var tsxReadStream = File.OpenRead( tsxPath ) ) {
                var document = XDocument.Load( tsxReadStream );
                var images = document.Root.Elements().Where( e => e.Name == "image" );
                foreach( var img in images) {
                    var source = img.Attributes().First( a => a.Name == "source" );
                    var imageFile = Path.Combine(tsxDir, Path.GetFileName( source.Value ));
                    File.Copy( source.Value, Path.Combine(projectDir, Project.Paths.TileSheets, imageFile), true );

                    source.Value = Path.Combine( Project.Paths.TileSheets, imageFile );
                }

                document.Save(Path.Combine( projectDir, Project.Paths.TileSheets, Path.GetFileName(tsxPath) ));
            }
        }
    }
}
