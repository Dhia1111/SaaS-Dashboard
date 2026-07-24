import React from "react";
import { 
  GridComponent, ColumnsDirective, ColumnDirective, 
  Filter, Inject, Page, Sort, Toolbar } from '@syncfusion/ej2-react-grids';
import { useNavigate } from 'react-router-dom';
import { useState,useEffect } from "react";
import {ClientSubscriptionAsync} from "../../Apis/Tenant"

export default function TenantPayments() {
  const Navigate=useNavigate();
  const[ClientsSubscrptions,setClientsSubscrptions]=useState([])
   const dynamicColumns = ClientsSubscrptions.length > 0 ? Object.keys(ClientsSubscrptions[0]) : [];

    const formatHeader = (text) => {
    return text.replace(/^./, (str) => str.toUpperCase()).trim();
  };
  useEffect(()=>{

     async function Fetch() {
      
      const res=await  ClientSubscriptionAsync();
    if(res.success){
      setClientsSubscrptions(res.data);
    }
    else{
 setClientsSubscrptions([]);
    }
     }
    

    

  })
  return (
    <div className="space-y-6 animate-in fade-in duration-200">
      <div>
        <h2 className="text-2xl font-extrabold text-slate-900 tracking-tight">Tenant Payment Management</h2>
        <p className="text-sm text-slate-500">Client subscriptions,Monitor processing bills, structured transactions, and direct invoice executions.</p>
      </div>

          
    <div className="space-y-6 animate-in fade-in duration-200">
      
      <div className="bg-white border border-slate-200/60 rounded-2xl shadow-sm p-4 overflow-hidden">
        {ClientsSubscrptions.length > 0 ? (
          <GridComponent 
            dataSource={ClientsSubscrptions || []} 
            allowPaging={true} 
            allowSorting={true} 
            allowFiltering={true}
            pageSettings={{ pageSize: 10 }}
            filterSettings={{ type: 'Menu' }}
            width="100%"
          >
            <ColumnsDirective>
              {dynamicColumns.map((key) => (
                <ColumnDirective 
                  key={key}
                  field={key} 
                  headerText={formatHeader(key)} 
                  width={key === 'Id' ? '100' : '150'}
                  textAlign={key === 'Id' ? 'Right' : 'Left'}
                />
              ))}
              <ColumnDirective 
                headerText="Actions" 
                width="200" 
                template={null} 
                textAlign="Center"
              />
            </ColumnsDirective>
            <Inject services={[Page, Sort, Filter, Toolbar]} />
          </GridComponent>
        ) : (
          <p className="text-center py-8 text-slate-400 font-medium text-sm">No systemic Prcing Cycles payloads found.</p>
        )}
      </div>
    </div>
  );
    </div>
  )
}