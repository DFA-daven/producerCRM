// ============================================================================
// <copyright file="EntitySearch.cs" company="Dairylea Cooperative Inc." >
//     Copyright 2010-2014 Dairylea Cooperative Inc. All rights reserved.
// </copyright>
//
// ============================================================================

/// <summary>File Name: EntitySearch
/// Description: Search for selected entities (Producers).
/// 
/// Original Author: unk
/// Created Date: unk 
///  
/// ************************Change History**************************** 
/// Altered Date:           Author Initials     -Change Description 
/// 2013-02                 DRN                 -Adding sorting to columns
/// 2013-03                 DRN                 -Refactored so that the sorting here
///                                              is passed to the entity's detail page
///                                              (via session variable) for use by the
///                                              search (previous/next) user controls.
/// 2013-04-05              DRN                 -Added "Entity" class for managing
///                                              the current Entity across pages and
///                                              controls.
/// 2014-07-11              DRN                 -Forked from DairyleaPortal.
/// </summary>
/// 
namespace BackEnd.EntitySearch
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using DairyleaDAL.Entities;
    using DairyleaDAL.Services;
    using System;

    /// <summary>UI For Entity Search
    /// </summary>
    public partial class EntitySearch : System.Web.UI.Page
    {   
        // Hack: empty controls to get this class Working
        internal GridView gdvProducer = new GridView();
        internal GridView gdvFieldPerson = new GridView();
        internal string lblProducers = "";
        internal string lblFieldPerson = "";
        internal string lblHeaderTotalCount = "";
        internal string lblError = "";

        #region Backing Fields
        private string _searchFilter = "";
        private bool _includeFieldPeople = false;
        private bool _includeProducers = false;
        private bool _includeInactive = false;

        /// <summary>Stores the producer division and number (8 digit, aka "member id")
        /// </summary>
        protected string PRODUCER_NUMBER = string.Empty;
        #endregion

        #region Properties
        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                _searchFilter = value;
                // FixMe: move this class to a ViewModel?
                //RaisePropertyChanged(() => SearchFilter);
            }
        }

        public bool IncludeInactive
        {
            get { return _includeInactive; }
            set
            {
                _includeInactive = value;
                // FixMe: move this class to a ViewModel?
                //RaisePropertyChanged(() => IncludeInactive);
            }
        }

        public bool IncludeFieldPeople
        {
            get { return _includeFieldPeople; }
            set
            {
                _includeFieldPeople = value;
                // FixMe: move this class to a ViewModel?
                //RaisePropertyChanged(() => IncludeFieldPeople);
            }
        }

        public bool IncludeProducers
        {
            get { return _includeProducers; }
            set
            {
                _includeProducers = value;
                // FixMe: move this class to a ViewModel?
                //RaisePropertyChanged(() => IncludeProducers);
            }
        }
        #endregion

        /// <summary>Stores the total count of each search result
        /// </summary>
        protected int totalCount = 0;

        /// <summary>Lookup Entity data for the given Search Word
        /// </summary>
        /// <param name="sender">Object that action was performed on</param>
        /// <param name="e">Arguments for Event</param>
        protected void StartSearch(object sender, EventArgs e)
        {
            try
            {
                // Reset the counter if they search again
                totalCount = 0;

                Search();

                // Checks if there is at least one search result
                if (totalCount > 0)
                {
                    // fix layout for different browsers

                    lblHeaderTotalCount = "(Matches: " + totalCount.ToString() + ")";
                }
                else
                {
                    InitializeEntities();
                    lblError = "No results found.";
                    lblHeaderTotalCount = string.Empty;
                }

                Session["search"] = _searchFilter;
                Session["includeInactive"] = _includeInactive.ToString();
            }
            catch (Exception ex)
            {
                //DairyleaPortal.Main.clsErrorLogging.LogError(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>Searches for all of the entities specified
        /// </summary>
        private void Search()
        {
            // Populates the producer search results if the producer entity is checked
            if (_includeProducers)
            {
                SearchProducer();
            }
            else
            {
                //SetEntityLabel(hylFilterProducer, "Producers");
                //producers.Style[HtmlTextWriterStyle.Display] = "none";
            }

            // Populates the Field Person search results if the Field Person entity is checked
            if (_includeFieldPeople)
            {
                SearchFieldPerson();
            }
            else
            {
                //SetEntityLabel(hylFilterFieldPerson, "Field Person");
                //fieldPerson.Style[HtmlTextWriterStyle.Display] = "none";
            }
        }

        /// <summary>Populates the producer repeater with possible matches
        /// </summary>
        private void SearchProducer()
        {
            try
            {
                string newSearch = _searchFilter;
                DataTable dt = EnterpriseDataService.SearchProducerByWord(newSearch, _includeInactive); // Stores and gets the producer search results

                AddExtendedPropertiesToDatatable(dt);
                dt.ExtendedProperties["ColumnsForKey"] = new int[] { 0, 1 };

                //Manage the GridView and Session Variable tables
                dt = ManageDatatable(this.Session, gdvProducer, dt, newSearch);

                // Checks if any search results have been found
                if (dt.Rows.Count > 0)
                {
                    BuildUserControlSessionList(dt, gdvProducer.ID);

                    //producers.Style[HtmlTextWriterStyle.Display] = "block";
                    lblProducers = "Matches: " + dt.Rows.Count.ToString();

                    //SetEntityLabel(hylFilterProducer, "Producers(" + dt.Rows.Count.ToString() + ")", "#producers");
                    totalCount += dt.Rows.Count;

                    // Checks if there is one producer found, then set producer number to send to the producerOverview.aspx page
                    if (dt.Rows.Count == 1)
                    {
                        PRODUCER_NUMBER = dt.Rows[0].ItemArray[0].ToString() + dt.Rows[0].ItemArray[1].ToString();

                        // Use the overloaded method and pass it false to prevent the thread abort exception 
                        Response.Redirect("ProducerOverview.aspx?memberId=" + PRODUCER_NUMBER, false);
                    }
                }
                else
                {
                    //producers.Style[HtmlTextWriterStyle.Display] = "none";
                }
            }
            catch (Exception ex)
            {
                // exceptions here often indicate missing DLLs.
                string innerException = ex.InnerException.Message;
                //DairyleaPortal.Main.clsErrorLogging.LogError(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>Populates the Field Person repeater with possible matches
        /// </summary>
        private void SearchFieldPerson()
        {
            try
            {
                //* *Change the search function**
                string newSearch = _searchFilter;
                DataTable dt = AdministrativeDataService.SearchFieldPersonByWord(newSearch); // Stores and gets the Field Person search results

                AddExtendedPropertiesToDatatable(dt);
                dt.ExtendedProperties["ColumnsForKey"] = new int[] { 0 };
                dt = ManageDatatable(this.Session, gdvFieldPerson, dt, newSearch);

                // Checks if any search results have been found
                if (dt.Rows.Count > 0)
                {
                    BuildUserControlSessionList(dt, gdvFieldPerson.ID);

                    //fieldPerson.Style[HtmlTextWriterStyle.Display] = "block";
                    lblFieldPerson = "Matches: " + dt.Rows.Count.ToString();

                    //SetEntityLabel(hylFilterFieldPerson, "Field Person(" + dt.Rows.Count.ToString() + ")", "#fieldPerson");
                    totalCount += dt.Rows.Count;
                }
                else
                {
                    //fieldPerson.Style[HtmlTextWriterStyle.Display] = "none";
                }
            }
            catch (Exception ex)
            {
                //DairyleaPortal.Main.clsErrorLogging.LogError(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>Creates the list of results for the session
        /// </summary>
        /// <param name="baseTable">The entity table, with all columns that will be used by the GridView.</param>
        /// <param name="entityGridViewName">The Session Variable name used by the User Control.</param>
        private void BuildUserControlSessionList(DataTable baseTable, string entityGridViewName)
        {
            DataTable sortedTable = baseTable.DefaultView.ToTable();
            List<string> list = new List<string>();
            int[] columnsForKey = baseTable.ExtendedProperties["ColumnsForKey"] as int[];
            string perRowKey = string.Empty;
            string entityListName = string.Empty;

            // use the GridView's name as the base for the Session Variable's name
            entityListName = entityGridViewName.Substring(3) + "List";

            for (int row = 0; row < baseTable.Rows.Count; row++)
            {
                perRowKey = string.Empty;

                // list.Add(fullTable.Rows[row][0].ToString() + fullTable.Rows[row][1].ToString());
                // if we need more than one column to form the query, concat them together
                for (int columnNumber = 0; columnNumber < columnsForKey.Count(); columnNumber++)
                {
                    if (columnsForKey[columnNumber] > -1)
                    {
                        // perRowKey = perRowKey + fullTable.Rows[row][columnNumber].ToString(); 
                        perRowKey = perRowKey + sortedTable.Rows[row][columnNumber].ToString();
                    }
                }

                list.Add(perRowKey);
            }

            // save the list to a session variable so the entity's detail page can retrieve the results
            Session[entityListName] = list;
        }

        /// <summary>Sets the entity label text and URL. If the URL is empty, applies a CSS Class.
        /// </summary>
        /// <param name="hyperLink">The target hyperlink control.</param>
        /// <param name="text">The text content.</param>
        /// <param name="href">The href.</param>
        private void SetEntityLabel(HyperLink hyperLink, string text, string href = "")
        {
            hyperLink.Text = text;
            hyperLink.NavigateUrl = href;

            if (hyperLink.NavigateUrl.Length > 0)
            {
                //hyperLink.RemoveCssClass("noResults");
            }
            else
            {
                //hyperLink.AddCssClass("noResults");
            }
        }

        /// <summary>Initializes all of the search entities
        /// </summary>
        void InitializeEntities()
        {
            // Hides all of the search result gridviews
            //producers.Style[HtmlTextWriterStyle.Display] = "none";
            //fieldPerson.Style[HtmlTextWriterStyle.Display] = "none";

            // Checks all of the entities as the default to search in
            //SelectAllEntities(true);

            // Sets the default entity label 
            //SetEntityLabel(hylFilterProducer, "Producers");
            //SetEntityLabel(hylFilterFieldPerson, "Field Person");
        }

        /// <summary>Manage the GridView and Session Variable tables.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="gridView">The GridView control that will hold the table data.</param>
        /// <param name="entityTable">A DataTable to be assigned to the GridView.</param>
        /// <param name="newSearch">The new search.</param>
        /// <returns>A DataTable, with Extended Properties set.
        /// </returns>
        /// <remarks>The GridView displays the table data, but the table and extended properties are held in a Session Variable.
        /// </remarks>
        public static DataTable ManageDatatable(HttpSessionState session, GridView gridView, DataTable entityTable, string newSearch = "")
        {
            DataTable bestTable = new DataTable();

            string lastSearch = string.Empty;

            // if the session variable version of the table exists load it, and the lastSearch that generated it
            // (on the first time through, the session variable will not exist yet)
            if (session[gridView.ID] != null)
            {
                bestTable = (DataTable)session[gridView.ID];
                lastSearch = bestTable.ExtendedProperties["LastSearch"].ToString();
            }

            // if this is a new search, initialize the new table and update the session variables and GridView
            if (lastSearch != newSearch)
            {
                bestTable = entityTable;

                // AddExtendedPropertiesToDatatable(bestTable);

                // Persist the table in the Session object.
                bestTable.ExtendedProperties["LastSearch"] = newSearch;
                session[gridView.ID] = bestTable;
            }

            // Bind the GridView control to the data source.
            gridView.DataSource = session[gridView.ID];
            gridView.DataBind();

            // now that it's bound, the pageCount exists...
            //UpdateGridviewPagerSettings(gridView, 1, gridView.PageCount);
            gridView.DataBind();

            return bestTable;
        }

        /// <summary>Add Extended Properties to the given DataTable, and populate them with default values.
        /// </summary>
        /// <param name="dataTable">DataTable to add properties to</param>
        private void AddExtendedPropertiesToDatatable(DataTable dataTable)
        {
            dataTable.ExtendedProperties.Add("LastSearch", string.Empty);

            int[] columnsForKey = new int[] { };
            dataTable.ExtendedProperties.Add("ColumnsForKey", columnsForKey);

            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnName == "Date")
                {
                    column.ExtendedProperties.Add("DefaultSortDirection", "DESC");
                    column.ExtendedProperties.Add("LastSortDirection", "DESC");
                }
                else
                {
                    column.ExtendedProperties.Add("DefaultSortDirection", "ASC");
                    column.ExtendedProperties.Add("LastSortDirection", string.Empty);
                }
            }
        }
    }

    /// <summary>Class for managing Entity Type.
    /// </summary>
    /// <remarks>This is used by the various pages in <c>DairyleaPortal/OverView</c> to keep track of the current entity.</remarks>
    internal static class Entity
    {
        /// <summary>
        /// Initializes static members of the <see cref="Entity" /> class.
        /// </summary>
        static Entity()
        {
            CurrentEntityParameter = string.Empty;
            CurrentEntityIdNumber = string.Empty;
            CurrentEntityContact = null;
        }

        /// <summary>Gets or sets the "type" of the Entity -- Producer, Customer, Hauler, FIPS, Area, Field Person
        /// </summary>
        /// <value>
        /// The current entity parameter.
        /// </value>
        public static string CurrentEntityParameter 
        { 
            get; 
            set; 
        }

        /// <summary>Gets or sets the ID number (as string) of the Entity currently displayed.
        /// </summary>
        public static string CurrentEntityIdNumber { get; set; }

        /// <summary>Gets or sets the current entity contact.
        /// </summary>
        /// <value>
        /// The current entity contact.
        /// </value>
        public static Contact CurrentEntityContact { get; set; }

        /// <summary>Entity Type
        /// </summary>
        public enum Type
        {
            Area,
            Customer,
            FieldPerson,
            FIPS,
            Hauler,
            Producer
        } 
    }
}