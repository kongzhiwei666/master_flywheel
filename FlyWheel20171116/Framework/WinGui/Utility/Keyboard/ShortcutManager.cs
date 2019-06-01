using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

//using AppExceptions = Microsoft.ApplicationBlocks.ExceptionManagement;
using Logger = Framework.Common.Logging;
using Framework.Core;

namespace Framework.WinGui.Utility.Keyboard
{
    /// <summary>
    /// Summary description for ShortcutManager.
    /// </summary>
    public class ShortcutManager : NameObjectCollectionBase
    {
        #region Private Members
        //		private static readonly log4net.ILog _log = Logger.Log.GetLogger(typeof(ShortcutManager));
        XmlSchema _schema;
        bool _validationErrorOccured;
        StringCollection _shortcutsToShow = new StringCollection();
        #endregion

        /// <summary>
        /// Constructor.  Loads in the schema and Settings.
        /// </summary>
        public ShortcutManager()
        {
            //using(Stream stream = Resource.Manager.GetStream("Resources.ShortcutSettings.xsd")) 
            using (Stream stream = Resource.Manager.GetStream("ShortcutSettings.xsd"))
            {
                _schema = XmlSchema.Read(stream, null);
                stream.Close();
            }
        }

        /// <summary>
        /// Access by Index.
        /// </summary>
        public System.Windows.Forms.Shortcut this[string commandName]
        {
            get
            {
                object shortcut = BaseGet(commandName);
                if (shortcut != null)
                    return (System.Windows.Forms.Shortcut)shortcut;
                else
                    return System.Windows.Forms.Shortcut.None;
            }
            set
            {
                this.BaseSet(commandName, (System.Windows.Forms.Shortcut)value);
            }
        }

        public bool IsDisplayed(string commandName)
        {
            return this._shortcutsToShow.Contains(commandName);
        }

        /// <summary>
        /// Loads the shortcut settings from an xml file.
        /// </summary>
        /// <param name="path"></param>
        public void LoadSettings(string path)
        {
            if (!File.Exists(path))
                throw new IOException("Settings File '" + path + "' Not Found!");

            using (FileStream stream = File.OpenRead(path))
            {
                LoadSettings(stream);
            }
        }

        /// <summary>
        /// Loads the shortcut settings from a stream.
        /// </summary>
        /// <param name="settingsStream"></param>
        public void LoadSettings(Stream settingsStream)
        {
            BaseClear();

            XmlDocument doc = LoadValidatedDocument(settingsStream);

            //mod by weimf
            //			if(_validationErrorOccured)
            //				throw new BanditApplicationException(Resource.Manager["RES_ExceptionInvalidShortcutSettingsFile"]);

            ShortcutSettings settings = DeserializeSettings(doc);
            PopulateFromSettings(settings);
        }

        private XmlDocument LoadValidatedDocument(Stream stream)
        {
            XmlDocument doc = new XmlDocument();

            XmlValidatingReader reader = new XmlValidatingReader(new XmlTextReader(stream));

            reader.Schemas.Add(_schema);
            reader.ValidationType = ValidationType.Schema;

            reader.ValidationEventHandler += new ValidationEventHandler(reader_ValidationEventHandler);
            _validationErrorOccured = false;

            doc.Load(reader);
            reader.Close();
            return doc;
        }

        private ShortcutSettings DeserializeSettings(XmlDocument doc)
        {
            //convert XML to objects 
            XmlNodeReader reader = new XmlNodeReader(doc);
            XmlSerializer serializer = new XmlSerializer(typeof(ShortcutSettings));
            ShortcutSettings settings = serializer.Deserialize(reader) as ShortcutSettings;
            reader.Close();
            return settings;
        }

        private void reader_ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            //mod by wei
            //			if(args.Severity == System.Xml.Schema.XmlSeverityType.Warning) {
            //				_log.Info("Shortcut Settings File validation warning: " + args.Message);
            //
            //			}
            //			else 
            if (args.Severity == System.Xml.Schema.XmlSeverityType.Error)
            {

                _validationErrorOccured = true;
                //mod by wei				
                //				_log.Error("Shortcut Settings File validation error: " + args.Message);
                //				AppExceptions.ExceptionManager.Publish(args.Exception);

            }
        }

        private void PopulateFromSettings(ShortcutSettings settings)
        {
            foreach (ShortcutSetting setting in settings.shortcut)
            {
                this[setting.commandName] = (System.Windows.Forms.Shortcut)Enum.Parse(typeof(System.Windows.Forms.Shortcut), setting.keys.ToString(), true);
                if (setting.display)
                    _shortcutsToShow.Add(setting.commandName);
            }
        }
    }
}