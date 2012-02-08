using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Windows;
using ExtendedPropertiesDocumentationTool.DataAccess;
using ExtendedPropertiesDocumentationTool.Entities;

namespace ExtendedPropertiesDocumentationTool
{
    public class MainWindowViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        DatabaseMetaData _dbMetaData;
        StoredProcedureMetaData _selectedSPMetadata;
        MetadataFacade _metaAccess = new MetadataFacade();
        

        public DelegateCommand<TableMetadata> SaveTableCommand { get; private set; }
        public DelegateCommand LoadDatabaseCommand { get; private set; }
        public DelegateCommand CreateWikiMarkupForTablesCommand { get; private set; }
        public DelegateCommand CreateWikiMarkupForSPsCommand { get; private set; }
        public DelegateCommand<TableMetadata> CreateWikiMarkupForSelectedTableCommand { get; private set; }
        public DelegateCommand OpenSqlConnectionBuilder { get; private set; }


        public DelegateCommand<StoredProcedureMetaData> CreateWikiMarkupForSelectedStoredProcedureCommand { get; private set; }
        public DelegateCommand<StoredProcedureMetaData> SaveStoreProcedureCommand { get; private set; }
        //

        public StoredProcedureMetaData SelectedStoredProcedure
          {
            get
            {
                return _selectedSPMetadata;
            }
            set
            {
                _selectedSPMetadata = value;
                if(_selectedSPMetadata != null)
                    _metaAccess.GetStoredProcedureMetaDataDetails(_selectedSPMetadata, ConnectionString);
                OnPropertyChanged("SelectedStoredProcedure");
            }
        }

        public DatabaseMetaData DatabaseMetaData
        {
            get
            {
                return _dbMetaData;
            }
            set
            {
                _dbMetaData = value;
                OnPropertyChanged("DatabaseMetaData");
            }
        }

        TableMetadata _selectedTableMetaData;
        public TableMetadata SelectedTable { 
            get 
            { 
                return _selectedTableMetaData; 
            } 
            set 
            {
                // TODO: Warn about unsaved changes.
                _selectedTableMetaData = value;
                if (_selectedTableMetaData != null)
                    _metaAccess.GetTableMetaDetails(_selectedTableMetaData, ConnectionString);               
                OnPropertyChanged("SelectedTable");
            } 
        }


        public string ConnectionString
        { 
            get 
            {
                string connStr = DocumentationTool.Default.ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                {
                    connStr = "Data Source=LOCALHOST;Initial Catalog=Master;Integrated Security=SSPI;";
                }
                return connStr;
            } 
            set 
            {
                DocumentationTool.Default.ConnectionString = value;
                DocumentationTool.Default.Save();
                OnPropertyChanged("ConnectionString");
            } 
        }

        public void ReloadDBMetaData(string connStr)
        {
            DatabaseMetaData = _metaAccess.GetMetaDataForDatabase(connStr);
            SelectedTable = DatabaseMetaData.Tables.FirstOrDefault();
            SelectedStoredProcedure = DatabaseMetaData.StoredProcedures.FirstOrDefault();
        }

        public void SaveChangesForTable(TableMetadata table, string connStr)
        {
            if(table != null)
                _metaAccess.SaveTableMetaData(table, connStr);
        }

        public MainWindowViewModel()
        {
            SaveTableCommand = new DelegateCommand<TableMetadata>(tmd =>
                {
                    SaveChangesForTable(tmd, ConnectionString);
                });
           
            LoadDatabaseCommand = new DelegateCommand( () =>
                {
                    try
                    {
                        ReloadDBMetaData(ConnectionString);
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Could not connect to the database, make sure you have entered the correct information: " + ex.ToString());
                    }
                    catch (Exception exx)
                    {
                        MessageBox.Show("Critical error somewhere: " + exx.ToString());
                    }
                });

            CreateWikiMarkupForTablesCommand = new DelegateCommand( () =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForTables(ConnectionString);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });

            CreateWikiMarkupForSPsCommand = new DelegateCommand( () =>
            {
                string result = _metaAccess.GenerateWikiMarkupForStoredProcedures(ConnectionString);
                ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                disp.Show(result);
            });

            CreateWikiMarkupForSelectedTableCommand = new DelegateCommand<TableMetadata>(tmd =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForTables(ConnectionString, tmd);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });

            CreateWikiMarkupForSelectedStoredProcedureCommand = new DelegateCommand<StoredProcedureMetaData>(spMetadata =>
                {
                    string result = _metaAccess.GenerateWikiMarkupForSelectedStoredProcedure(spMetadata, ConnectionString);
                    ModalWindows.WikiMarkupDisplay disp = new ModalWindows.WikiMarkupDisplay();
                    disp.Show(result);
                });

            SaveStoreProcedureCommand = new DelegateCommand<StoredProcedureMetaData>(spMetadata =>
                {
                    _metaAccess.SaveStoredProcedureMetadata(_selectedSPMetadata, ConnectionString);
                });


            OpenSqlConnectionBuilder = new DelegateCommand(() =>
                {
                    ConnectionStringCreatorGUI.SqlConnectionString initialConnStr;

                    try
                    {
                        initialConnStr = new ConnectionStringCreatorGUI.SqlConnectionString(ConnectionString);
                    }
                    catch (Exception)
                    {
                        initialConnStr = new ConnectionStringCreatorGUI.SqlConnectionString();
                    }
                 
                    Window win = new ConnectionStringCreatorGUI.ConnectionStringBuilderWindow(initialConnStr, returnConnBuilder =>
                    {
                        ConnectionString = returnConnBuilder.ToString();
                    });

                    win.Show();

                }
            );

        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));      
        }
        
    }
}
