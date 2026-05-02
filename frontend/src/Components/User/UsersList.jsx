import React from 'react';
import { 
  GridComponent, ColumnsDirective, ColumnDirective, 
  Filter, Inject, Page, Sort, Toolbar 
} from '@syncfusion/ej2-react-grids';
import { ButtonComponent } from '@syncfusion/ej2-react-buttons';
import { Edit2, Trash2, Info } from 'lucide-react';
import { useState,useEffect } from 'react';
import {ListAsync as UsersListAsync} from '../../Apis/Users.js';

// Registering Syncfusion license (Required for production)
// import { registerLicense } from '@syncfusion/ej2-base';
// registerLicense('YOUR_LICENSE_KEY');
 
 
const UserDashboard = () => {

  async  function GetUsers(){
        const users = await UsersListAsync();
        console.log("Fetched users:", users);
        setUsers(users||[]);
    }

  // Sample Data
  const [Users, setUsers] = useState([]);
  // Custom Template for Action Buttons
  const actionTemplate = (props) => {
  return (
    <div className="flex items-center gap-3 justify-center">
      {/* View Details Button - Blue/Indigo */}
      <button 
        onClick={() => console.log("Details", props.id)}
        className="w-9 h-9 rounded-full bg-indigo-50 text-primary flex items-center justify-center hover:bg-primary hover:text-white transition-all duration-200 group"
        title="View Details"
      >
        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
        </svg>
      </button>

      {/* Edit Button - Slate/Gray */}
      <button 
        onClick={() => console.log("Edit", props.id)}
        className="w-9 h-9 rounded-full bg-slate-100 text-slate-600 flex items-center justify-center hover:bg-slate-800 hover:text-white transition-all duration-200"
        title="Edit User"
      >
        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
        </svg>
      </button>

      {/* Delete Button - Red/Danger */}
      <button 
        onClick={() => console.log("Delete", props.id)}
        className="w-9 h-9 rounded-full bg-red-50 text-red-600 flex items-center justify-center hover:bg-red-600 hover:text-white transition-all duration-200"
        title="Delete User"
      >
        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
        </svg>
      </button>
    </div>
  );
};
useEffect(()=>{
  GetUsers();
},[])
const formatHeader = (text) => {
    return text.replace(/^./, (str) => str.toUpperCase())
      .trim();
  };

  // Get keys from the first object in your array
  const dynamicColumns = Users.length > 0 ? Object.keys(Users[0]) : [];
  return (
    <div className="w-full bg-white rounded-card shadow-card pt-[100px]  mt-[24px]">
      <h2 className="text-2xl font-bold font-heading mb-6">User Management Dashboard</h2>
      
      {Users.length > 0 &&<GridComponent 
        dataSource={Users} 
        allowPaging={true} 
        allowSorting={true} 
        allowFiltering={true}
        pageSettings={{ pageSize: 10 }}
        filterSettings={{ type: 'Menu' }} // Excel-style filtering
      >
        <ColumnsDirective>
           {/* Dynamic Mapping */}
        {dynamicColumns.map((key) => (
          <ColumnDirective 
            key={key}
            field={key} 
            // Apply our string manipulation here
            headerText={formatHeader(key)} 
            width={key === 'UserID' ? '100' : '150'}
            textAlign={key === 'UserID' ? 'Right' : 'Left'}
          />
        ))}
          <ColumnDirective 
            headerText="Actions" 
            width="250" 
            template={actionTemplate} 
            textAlign="Center"
          />
        </ColumnsDirective>
        
        {/* Injecting required modules for the Grid */}
        <Inject services={[Page, Sort, Filter, Toolbar]} />
      </GridComponent>}
      {Users.length === 0 && <p className="text-center text-gray-500">No users found.</p>}
    </div>
  );
};

export default UserDashboard;