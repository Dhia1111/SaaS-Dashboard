import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  GridComponent, ColumnsDirective, ColumnDirective, 
  Filter, Inject, Page, Sort, Toolbar 
} from '@syncfusion/ej2-react-grids';
import { GetPricingCyclesAsync } from '../../Apis/PricingCycles.js';

export default function PricingCyclesSettings() {
  const [pricingCycles, setPricingCycles] = useState([]);
  const navigate = useNavigate();

 
  useEffect(() => {
     async function GetPricingCycles() {
    const response = await GetPricingCyclesAsync();
    setPricingCycles(response.data || []);
  }

    GetPricingCycles();
  }, []);

  const actionTemplate = (props) => {
    return (
      <div className="flex items-center gap-3 justify-center">
        <button 
          onClick={() => console.log("Details", props.id)}
          className="w-8 h-8 rounded-full bg-indigo-50 text-indigo-600 flex items-center justify-center hover:bg-indigo-600 hover:text-white transition-all duration-200"
          title="View Details"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
        </button>

        <button 
          onClick={() => console.log("Edit", props.id)}
          className="w-8 h-8 rounded-full bg-slate-100 text-slate-600 flex items-center justify-center hover:bg-slate-800 hover:text-white transition-all duration-200"
          title="Edit User"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
        </button>

        <button 
          onClick={() => console.log("Delete", props.id)}
          className="w-8 h-8 rounded-full bg-red-50 text-red-600 flex items-center justify-center hover:bg-red-600 hover:text-white transition-all duration-200"
          title="Delete User"
        >
          <svg className="w-4 h-4" fill="none" stroke="currentColor" strokeWidth="2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
    );
  };

  const formatHeader = (text) => {
    return text.replace(/^./, (str) => str.toUpperCase()).trim();
  };

  const dynamicColumns = pricingCycles.length > 0 ? Object.keys(pricingCycles[0]) : [];

  return (
    <div className="space-y-6 animate-in fade-in duration-200">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      
        <button 
          onClick={() => navigate("add")}
          className="inline-flex items-center justify-center px-4 py-2.5 bg-indigo-600 text-white font-bold text-sm rounded-xl shadow-lg shadow-indigo-100 hover:bg-indigo-700 transition-all outline-none"
        >
          Add New Pricing Cycle
        </button>
      </div>

      <div className="bg-white border border-slate-200/60 rounded-2xl shadow-sm p-4 overflow-hidden">
        {pricingCycles.length > 0 ? (
          <GridComponent 
            dataSource={pricingCycles || []} 
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
                template={actionTemplate} 
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
}