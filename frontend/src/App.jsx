import Main from './Components/Platform/Main'
import Contact from './Components/Platform/Contact'
import PlatformLayout from './Components/Platform/PlatformLayout'
import LegalPage from './Components/Platform/Legal'
import {  Route } from 'react-router-dom'
import { createBrowserRouter, createRoutesFromElements } from 'react-router-dom'
import SignupFlow from './Components/Platform/SignUp'
import { RouterProvider } from 'react-router-dom'
import LogIn from './Components/Platform/LogIn'
import UsersList from './Components/User/UsersList'
import { User } from './Components/User/User'
import AddUser from './Components/User/AddUser'
import {Loader as AddUserLoader} from './Components/User/AddUser'
import VerifyUser from './Components/User/VerifyUser'
import  UserAuth  from './Components/UserAuth/UserAuth.jsx'
import UserLogIn from './Components/UserAuth/LogIn.jsx'
import SingInOptions from './Components/Platform/SignInOptions.jsx'
import DashboardLayout from './Components/DashBoard/Dashboard.jsx'
import BusinessAnalysis from './Components/BusinessAnalysis/BusinessAnalysis.jsx'
import TenantPayments from './Components/TenantPayments/TenantPayments.jsx'
import SubscriptionSettings from './Components/SubscriptionSettings/SubscriptionSettings.jsx'
import TenantPermissionList from './Components/EndPointsPermissionsManagement/PermissionsList.jsx';
import {PermissionLayout} from './Components/EndPointsPermissionsManagement/PermissionLayout.jsx'
import AddNewPermission from './Components/EndPointsPermissionsManagement/AddNewPermission.jsx'
import './App.css'

const router = createBrowserRouter(
  createRoutesFromElements(
    <Route path="/" element={<PlatformLayout/>} >
    <Route index element={<Main/>} />
    <Route path="contact" element={<Contact />} />
    <Route path="terms" element={<LegalPage type="terms" />} />
    <Route path="privacy" element={<LegalPage type="privacy" />} />
    <Route path="signin-options" element={<SingInOptions />} />
    <Route path="login" element={<LogIn />} />   
    <Route path="signup" element={<SignupFlow />} />
      <Route path="user-auth" element={<UserAuth/>}>
        <Route path="login" element={<UserLogIn />} />
       </Route>
    <Route path="dashboard" element={<DashboardLayout />} >
      <Route path="user" element={<User/>} >
        <Route index element={<UsersList />} />
        <Route path="add" element={<AddUser />} loader={AddUserLoader} />
        <Route path="verify" element={<VerifyUser />} />
   </Route>
   
     
     <Route path="permissions" element={<PermissionLayout />} >
     <Route  index element={<TenantPermissionList />} />
     <Route path="add" element={<AddNewPermission />} />
     </Route>
     <Route path="analytics" element={<BusinessAnalysis />} />
  <Route path="payments" element={<TenantPayments />} />
  <Route path="subscriptions" element={<SubscriptionSettings />} />
  <Route path="authorization" element={<SubscriptionSettings />} />
        </Route>

    </Route>
  )
)

function App() {
 
 return <RouterProvider router={router} />
  }

export default App
