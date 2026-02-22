import Main from './Components/Platform/Main'
import Contact from './Components/Platform/Contact'
import PlatformLayout from './Components/Platform/PlatformLayout'
import LegalPage from './Components/Platform/Legal'
import {  Route } from 'react-router-dom'
import { createBrowserRouter, createRoutesFromElements } from 'react-router-dom'
import SignupFlow from './Components/Platform/SignUp'
import { RouterProvider } from 'react-router-dom'
import LogIn from './Components/Platform/LogIn'
import './App.css'

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route path="/" element={<PlatformLayout/>} >
      <Route index element={<Main/>} />
     <Route path="contact" element={<Contact />} />
      <Route path="terms" element={<LegalPage type="terms" />} />
     <Route path="privacy" element={<LegalPage type="privacy" />} />
          <Route path="login" element={<LogIn />} />
          <Route path="signup" element={<SignupFlow />} />


    </Route>
  )
)

function App() {
 
 return <RouterProvider router={router} />
  }

export default App
