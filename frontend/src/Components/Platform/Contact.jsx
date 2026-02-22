import { contactContent } from '../../assets/Data/Platform/Contect';

export default function Contact() {
  const { header, supportCards, formOptions } = contactContent;

  return (
    <div className="min-h-screen bg-slate-50 font-body text-slate-900">
      <div className="max-w-7xl mx-auto px-8 py-section">
        
        {/* Header - Dynamic */}
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h1 className="text-4xl md:text-5xl font-extrabold font-heading mb-6">
            {header.title.replace(header.highlight, '')} 
            <span className="text-primary">{header.highlight}</span>
          </h1>
          <p className="text-lg text-secondary">
            {header.description}
          </p>
        </div>

        <div className="grid lg:grid-cols-3 gap-12">
          
          {/* Support Cards - Dynamic Loop */}
          <div className="lg:col-span-1 space-y-6">
            {supportCards.map((card, index) => (
              <div 
                key={index}
                className={`p-card rounded-card shadow-card border transition-base ${
                  card.type === 'featured' 
                    ? 'bg-primary text-white border-transparent' 
                    : 'bg-white border-slate-100'
                }`}
              >
                <h3 className="font-heading font-bold text-lg mb-2">{card.title}</h3>
                <p className={`text-sm mb-4 ${card.type === 'featured' ? 'opacity-90' : 'text-secondary'}`}>
                  {card.description}
                </p>
                {card.type === 'featured' ? (
                  <button className="bg-white text-primary px-4 py-2 rounded-button text-xs font-bold uppercase tracking-wider hover:bg-slate-50">
                    {card.linkText}
                  </button>
                ) : (
                  <a href={card.linkHref} className="text-primary font-bold text-sm hover:underline">
                    {card.linkText} &rarr;
                  </a>
                )}
              </div>
            ))}
          </div>

          {/* Form Side */}
          <div className="lg:col-span-2 bg-white p-card rounded-card shadow-card border border-slate-100">
            <form className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700">Full Name</label>
                <input type="text" className="w-full px-4 py-3 rounded-button border border-slate-200 focus:border-primary outline-none" />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-bold text-slate-700">Work Email</label>
                <input type="email" className="w-full px-4 py-3 rounded-button border border-slate-200 focus:border-primary outline-none" />
              </div>
              <div className="md:col-span-2 space-y-2">
                <label className="text-sm font-bold text-slate-700">Project Type</label>
                <select className="w-full px-4 py-3 rounded-button border border-slate-200 focus:border-primary outline-none bg-transparent">
                  {formOptions.map((option) => (
                    <option key={option} value={option}>{option}</option>
                  ))}
                </select>
              </div>
              <div className="md:col-span-2 space-y-2">
                <label className="text-sm font-bold text-slate-700">Message</label>
                <textarea rows="4" className="w-full px-4 py-3 rounded-button border border-slate-200 focus:border-primary outline-none"></textarea>
              </div>
              <button className="bg-primary text-white px-8 py-4 rounded-button font-bold shadow-lg shadow-indigo-100">
                Send Message
              </button>
            </form>
          </div>

        </div>
      </div>
    </div>
  );
}