import React from 'react'
import {Outlet} from 'react-router-dom'
import NavBar from './NavBar.jsx'
export function User() {
  return (
    <div>
      <NavBar Links={[{title:"Users",value:"/user"},{title:"Add User",value:"/user/add"}]} />
      <Outlet />
    </div>
  )
}