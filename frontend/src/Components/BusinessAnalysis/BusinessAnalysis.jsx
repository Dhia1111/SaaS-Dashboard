import React from "react";
import { useEffect,useState } from "react";
import DiscoveryAndConversion from './DiscoveryAndConversion/DiscoveryAndConversion'
import {MarkettingPlaataformsLeadsContributions ,MarkettingPlaataformsConverssionContributions} from '../../Apis/BusinessAnalyses'
export default function BusinessAnalysis() {

  const [LeadsContributions,setLeadsContributions]=useState([]);
  const [ConverssionContributions,setConverssionContributions]=useState([]);
    
  
  useEffect(()=>{
  
      async function   loadData(){
          const [leads,converssions]= await Promise.all([MarkettingPlaataformsLeadsContributions(),MarkettingPlaataformsConverssionContributions()])
          if(leads.success&&converssions.success){
            setLeadsContributions(leads.data);
            setConverssionContributions(converssions.data);
          }
          else{
            setLeadsContributions([]);
            setConverssionContributions([]);
            
          }
      }
  
      loadData();
  
  
  },[])
  
  return (
    <div className="space-y-6 animate-in fade-in duration-200">
      <div className="min-w-[300px] ">
        <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Business Analysis</h2>
        <p className="text-sm text-slate-500">Track structural data matrices, operational conversions, and tenant growth metrics.</p>
      </div>

      <div className="  flex justify-around  flex-wrap  gap-10    ">
        <div className=" min-w-[300px]  flex-column  bg-white  items-center  justify-center p-8 flex-1">
          <div className="flex   justify-between px-5 py-6 w-full px-10 border-b border-slate-100 pb-3">
              <h3 className="font-bold text-slate-900 text-sm">Leads</h3>
            <span className="text-xs font-bold text-indigo-600 bg-indigo-50 px-2.5 py-1 rounded-lg">All Times</span>
          </div>
          
                               <DiscoveryAndConversion data={LeadsContributions} />


         </div>

        <div className=" min-w-[300px] flex-1 bg-white p-8 border border-slate-200/60 rounded-2xl shadow-sm space-y-4">
          <div className="flex justify-between items-center border-b border-slate-100 pb-3">
            <h3 className="font-bold text-slate-900 text-sm">Tenant Growth Trajectory</h3>
            <span className="text-xs font-bold text-indigo-600 bg-indigo-50 px-2.5 py-1 rounded-lg">Monthly</span>
          </div>
          <div className="h-48    bg-slate-50 border border-dashed border-slate-200 rounded-xl flex items-center justify-center text-xs text-slate-400 font-bold uppercase tracking-wider">
            [Growth Over Time Linear Plot Context]
          </div>

        </div>
        
    <div className=" min-w-[300px]  flex-column  bg-white  items-center  justify-center p-8 flex-1">
          <div className="flex   justify-between px-5 py-6 w-full px-10 border-b border-slate-100 pb-3">
              <h3 className="font-bold text-slate-900 text-sm">Converssion</h3>
            <span className="text-xs font-bold text-indigo-600 bg-indigo-50 px-2.5 py-1 rounded-lg">All Times</span>
          </div>
          
                               <DiscoveryAndConversion data={ConverssionContributions} />


         </div>
      </div>
    </div>
  );
}