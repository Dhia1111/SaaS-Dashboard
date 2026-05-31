import React from 'react'
import {Outlet} from 'react-router-dom'
import NavBar from '../User/NavBar'

export default function UserAuth() {
  return (
    <div>
       <Outlet />
    </div>
  )
}