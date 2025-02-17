﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using FYP_IncentiveMechanismSimulatorMVP.Model;
using FYP_IncentiveMechanismSimulatorMVP.Utils;

namespace FYP_IncentiveMechanismSimulatorMVP.ApplicationLogic
{
    /*
     * Manages the list of dynamic object created from IncentiveSchemes.py
     * 1-to-1 Federation and Scheme 
     */
    public class SchemeManager
    {
        public List<dynamic> PythonSchemeList { get; set; }
        public PythonInterface PythonInterfaceReference { get; set; }
 
        public SchemeManager()
        {
            this.PythonSchemeList = new List<dynamic>();
        }
        /*
        #region C-sharp implementation
        public void LoadSchemes()
        {
            Console.WriteLine("Loading Schemes ......");
            Utils.IOManager tempIO = new Utils.IOManager();
            List<string> schemeTypeList = new List<string>();

            string source = @"..\..\Schemes";
            Console.WriteLine(source);
            string[] files = Directory.GetFiles(source, "*.cs");
            
            for(int i=0; i < files.Length; i++)
            {
                schemeTypeList.Add(Path.GetFileName(files[i]));
            }

            int count = 0;
            string schemeDirectoryNamespace = "FYP_IncentiveMechanismSimulatorMVP.Schemes";
            foreach (string s in schemeTypeList)
            {
                string fullNamespace = schemeDirectoryNamespace + "." + s.Replace(".cs", "");
                var obj = Activator.CreateInstance(null, fullNamespace);
                Scheme schemeObj = (Scheme) obj.Unwrap();
                schemeObj.SchemeId = count;
                schemeObj.SchemeName = s.Replace(".cs", "");
                Console.WriteLine("Creating instance of {0}", s.Replace(".cs", ""));
                this.SchemeList.Add(schemeObj);
                count++;
            }
        }
        #endregion
        */
        /*
         * Refer to PythonInterface creation of python object
         * Main idea: Reference to a python class of specific incentive scheme
         *            Upon trigger or access to this reference, then create the python class object (new object from reference) and link to Federation via index-linking
         */
        public void BuildFederationSchemeList(int numFed, int num_players)
        {
            //reinit
            this.PythonSchemeList.Clear();

            //TODO random assignment: current assignment linear
            int random = 0;
            for(int i=0; i<numFed; i++)
            {
                if(random==0)
                    this.PythonSchemeList.Add(this.PythonInterfaceReference.CreatePyScheme(i%this.PythonInterfaceReference.IncentiveSchemenameList.Count, num_players));
            }
            //for console display purposes
            for(int i=0; i < numFed; i++)
            {
                Console.WriteLine("Index " + (i+1) + " assigned " + this.PythonSchemeList[i].Name);
            }
        }

        //for fixed params
        public void BuildFederationSchemeList(int numFed, int num_players, List<int> fedSchemeSplit)
        {
            //reinit
            this.PythonSchemeList.Clear();

            //TODO random
            int random = 0;
            for (int i = 0; i < numFed; i++)
            {
                if (random == 0)
                    this.PythonSchemeList.Add(this.PythonInterfaceReference.CreatePyScheme(fedSchemeSplit[i], num_players));
            }

            for (int i = 0; i < numFed; i++)
            {
                Console.WriteLine("Index " + (i + 1) + " assigned " + this.PythonSchemeList[i].Name);
            }
        }
        /*
         * Returns a list of payoff based on participants' contribution. 
         */
        public List<double> CalculatePayoff(double budgetToAllocate, int federationId, List<Player> participantList, double data_qual_weight, double data_quant_weight)
        {
            //List<Tuple<int, double>> tempList = new List<Tuple<int, double>>();
            //build list to pass to PythonInterfaceObject
            List<Player> playerList = new List<Player>();
            for(int i=0; i<Simulation.Instance.simulationSettings.NUM_OF_PLAYERS; i++)
            {
                Player tempPlayer = new Player(i + 1);
                int index = participantList.FindIndex(p => p.Pid == (i + 1));
                if (index != -1)
                {
                    tempPlayer = participantList[index];
                }

                playerList.Add(tempPlayer);
            }
            Console.WriteLine(String.Format("Federation {0} - Budget to allocate -${1} : Scheme {2}", federationId, budgetToAllocate,this.PythonSchemeList[federationId-1]));
            List<double> payoff_list = this.PythonInterfaceReference.Calculate_Payoff(budgetToAllocate, this.PythonSchemeList[federationId - 1], playerList, data_qual_weight,data_quant_weight);
            return payoff_list;
        }

    }
}
