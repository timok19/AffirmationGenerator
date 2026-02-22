import LinkedInLink from "./LinkedInLink";
import GithubLink from "./GithubLink";
import Separator from "./Separator.tsx";

function Footer() {
  return (
    <footer
      className="glass px-6 py-2 h-12 rounded-full flex items-center gap-6 text-gray-800/80 hover:text-gray-900 transition-colors mt-8">
      
      <div className="flex items-center gap-2 hover:scale-105 transition-transform">
        <span className="font-semibold md:text-sm text-[0.8rem] text-center">Created by Temirkhan Amanzhanov</span>
      </div>

      <Separator/>

      <div className="flex gap-4">
        <LinkedInLink/>
        <GithubLink/>
      </div>

    </footer>
  );
}

export default Footer;
