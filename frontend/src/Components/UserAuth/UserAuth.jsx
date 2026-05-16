import React from 'react'
import {Outlet} from 'react-router-dom'
import NavBar from '../User/NavBar'

export default function UserAuth() {
  return (
    <div>
      <NavBar Links={[{title:"Users",value:"/user"},{title:"Login",value:"/user/auth/login"}]} />
      <Outlet />
    </div>
  )
}