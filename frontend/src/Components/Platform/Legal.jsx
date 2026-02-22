import { termsContent, privacyContent } from '../../assets/Data/Platform/Content';

export default function Legal({ type }) {
  const data = type === 'terms' ? termsContent : privacyContent;
  const title = type === 'terms' ? 'Terms of Service' : 'Privacy Policy';

  return (
    <div className="min-h-screen bg-white font-body text-slate-900">
      {/* Simple Header */}
      <header className="bg-slate-50 border-b border-slate-100 py-16 px-8">
        <div className="max-w-3xl mx-auto">
          <h1 className="text-4xl font-extrabold font-heading mb-4">{title}</h1>
          <p className="text-secondary text-sm italic">Last Updated: {data.lastUpdated}</p>
        </div>
      </header>

      {/* Content */}
      <main className="max-w-3xl mx-auto px-8 py-16">
        <div className="prose prose-slate prose-indigo">
          {data.sections.map((section, index) => (
            <div key={index} className="mb-12">
              <h2 className="text-xl font-bold font-heading text-slate-900 mb-4">
                {section.title}
              </h2>
              <p className="text-secondary leading-relaxed">
                {section.content}
              </p>
            </div>
          ))}
        </div>
        
        {/* Contact CTA */}
        <div className="mt-16 p-card bg-slate-50 rounded-card border border-slate-100 text-center">
          <p className="text-sm text-secondary">
            Have questions about our {title.toLowerCase()}? 
            <a href="/contact" className="text-primary font-bold ml-1 hover:underline">Contact Legal Support</a>
          </p>
        </div>
      </main>
    </div>
  );
}