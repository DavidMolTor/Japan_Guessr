﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JapanGuessr.Properties {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JapanGuessr.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap _default {
            get {
                object obj = ResourceManager.GetObject("_default", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Busca un recurso adaptado de tipo System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap background {
            get {
                object obj = ResourceManager.GetObject("background", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El directorio configurado no se ha encontrado.
        ///Elija un directorio válido..
        /// </summary>
        internal static string Main_textDirectoryError {
            get {
                return ResourceManager.GetString("Main_textDirectoryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Distancia al objetivo: [DIST].
        /// </summary>
        internal static string Main_textDistanceToTarget {
            get {
                return ResourceManager.GetString("Main_textDistanceToTarget", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No quedan imágenes en la carpeta actual por procesar.
        ///Vuelva a iniciar el juego en el modo deseado..
        /// </summary>
        internal static string Main_textNoPicturesLeft {
            get {
                return ResourceManager.GetString("Main_textNoPicturesLeft", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No hay ninguna ubicación seleccionada.
        ///Por favor, seleccione una ubicación antes de continuar..
        /// </summary>
        internal static string Main_textSelectLocation {
            get {
                return ResourceManager.GetString("Main_textSelectLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No se ha encontrado información de GPS en la imagen.
        ///¿Te gustaría añadirla?.
        /// </summary>
        internal static string Main_textSetInfoGPS {
            get {
                return ResourceManager.GetString("Main_textSetInfoGPS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No se ha configurado el directorio de imágenes.
        ///¿Quieres seleccionar un directorio diferente al predeterminado?.
        /// </summary>
        internal static string Main_textSetPicturesPath {
            get {
                return ResourceManager.GetString("Main_textSetPicturesPath", resourceCulture);
            }
        }
    }
}
