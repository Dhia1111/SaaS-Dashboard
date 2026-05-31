import React from 'react'
import {Outlet} from 'react-router-dom'
 
export function PermissionLayout() {
  return (
    <div>
      <Outlet />
    </div>
  )
}