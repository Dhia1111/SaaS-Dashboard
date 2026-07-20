
import { Link, useNavigate } from "react-router-dom"

export default function NavBar({Links=[{title:"",value:""}]}){
 const navigate =useNavigate()
return(
     <nav className="px-6 py-4 bg-white/80 backdrop-blur-md fixed w-full z-10 border-b border-gray-100">
        <div className="max-w-7xl mx-auto flex justify-between items-center">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 bg-gradient-to-br from-primary to-secondary rounded-lg"></div>
             <Link to="/" className="flex items-center gap-2"> 
           <span className="text-xl font-bold bg-gradient-to-r from-primary to-secondary bg-clip-text text-transparent">
              BillFlow
            </span>
            </Link>
          </div>
          <ul className="hidden md:flex items-center gap-8 text-gray-600 list-none">
            {Links.map((link, index) => (
              <li key={index}>
                <a href={link.value} className="hover:text-primary transition">
                  {link.title}
                </a>
              </li>
            ))}
          </ul>
          <div className="flex items-center gap-4">
            <button className="px-4 py-2 text-gray-600 hover:text-primary transition"onClick={()=>{
       var store=localStorage.getItem("userInfo");
        if(!store){
             store=localStorage.getItem("tenantInfo");
           }
      const isLogedIn= store!=null;
          var ref= !isLogedIn? "/signin-options":"/dashboard"   
       return   navigate(ref);

          }}>

          <span  className="hover:text-primary transition px-20"  >
                   Acount
                </span>
            </button>
          
          </div>
        </div>
      </nav>


)
    
}