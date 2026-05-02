import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Provider } from 'react-redux';
import {store} from "./store"
import './index.css'
 import '../node_modules/@syncfusion/ej2-buttons/styles/tailwind.css';
 import "../node_modules/@syncfusion/ej2-base/styles/material.css";
import "../node_modules/@syncfusion/ej2-calendars/styles/material.css";
import "../node_modules/@syncfusion/ej2-dropdowns/styles/material.css";
import "../node_modules/@syncfusion/ej2-inputs/styles/material.css";
import "../node_modules/@syncfusion/ej2-navigations/styles/material.css";
import "../node_modules/@syncfusion/ej2-popups/styles/material.css";
import "../node_modules/@syncfusion/ej2-splitbuttons/styles/material.css";
import "../node_modules/@syncfusion/ej2-notifications/styles/material.css";
import "../node_modules/@syncfusion/ej2-react-grids/styles/material.css";
import './styles/GridCustom.css';
import App from './App.jsx'
import { registerLicense } from '@syncfusion/ej2-base';

// 2. Inject your key here (Get this from your Syncfusion dashboard)
registerLicense('Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXtcdXVTR2lYU0N1W0dWYEo=');

createRoot(document.getElementById('root')).render(
    <StrictMode>
      
 <Provider store={store}>
    <App />
  </Provider >
    </StrictMode>,
)
