import {Outlet} from 'react-router-dom'
import Navbar from './NavBar'
import Links from "../../assets/Data/Platform/PagesLinks.js"
import {footerContent as contact}  from '../../assets/Data/Platform/Content.js'

function PlatformLayout() {
  return (
    <div>
      <Navbar Links={Links}  />
      <Outlet />
   <footer className="border-t border-slate-100 py-12 px-8 bg-white mt-auto">
      <div className="max-w-7xl mx-auto">
        {/* Top Section: Branding & Info */}
        <div className="grid md:grid-cols-2 gap-8 mb-8">
          <div className="space-y-4">
            <div className="text-xl font-bold tracking-tighter text-primary font-heading">
              BillFlow<span className="text-slate-400">.io</span>
            </div>
            <p className="text-secondary max-w-xs text-sm leading-relaxed">
              Simplifying Layer 2 billing so you can focus on building features, not subscription logic.
            </p>
          </div>
          
          {/* Contact Info Grid */}
          <div className="grid sm:grid-cols-2 gap-4 text-sm text-secondary">
            <div className="space-y-1">
              <span className="font-bold text-slate-900 block uppercase text-[10px] tracking-widest">Email Us</span>
              <a href={`mailto:${contact.email}`} className="hover:text-primary transition-base">
                {contact.email}
              </a>
            </div>
            <div className="space-y-1">
              <span className="font-bold text-slate-900 block uppercase text-[10px] tracking-widest">Call Us</span>
              <p>{contact.phone}</p>
            </div>
          </div>
        </div>

        <div className="pt-8 border-t border-slate-50 flex flex-col md:flex-row justify-between items-center gap-6">
          {/* Copyright */}
          <div className="text-xs text-secondary opacity-70">
            {contact.copyright}
          </div>

          {/* Links & Socials */}
          <div className="flex flex-wrap justify-center gap-x-8 gap-y-2 text-xs font-semibold text-slate-600">
            {contact.links.map((link) => (
              <a key={link.label} href={link.href} className="hover:text-primary transition-base">
                {link.label}
              </a>
            ))}
            <div className="h-4 w-px bg-slate-200 hidden md:block"></div>
            {contact.socials.map((social) => (
              <a key={social.label} href={social.href} className="hover:text-primary transition-base">
                {social.label}
              </a>
            ))}
          </div>
        </div>
      </div>
    </footer>
    </div>
  )
}
export default PlatformLayout