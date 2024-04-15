## What is Sam?

Sam is an AI program I wrote to crawl the web for job postings, find good matches based on my description of what I'm looking for, and report its findings to me.

It uses:
* OpenAI and Microsoft's SemanticKernel library for the core LLM logic
* PuppeteerSharp to browse the web
* HtmlAgilityPack to (very crudely) pre-process the HTML, reducing it to something I can have a secondary LLM summarize for Sam. (Token limits, man!)

## How to Run

* Clone or download this project
* Copy `FindMeAJob/appsettings.json.sample` to `FindMeAJob/appsettings.json`, and it edit it to suit your needs
  * Especially important: you'll need an OpenAI API key!
* Run it!

Notes: the default config uses ChatGPT 3.5, because it's waaaaaay cheaper than ChatGPT 4, but my code often hits the token limit of 3.5, causing it to crash. You'll get better results with ChatGPT 4, but definitely keep an eye on your spending: I'd say it costs about $0.50 per run, depending on how many jobs it finds and writes cover letters for.

## Sample Run
```
Ben > Hey, Sam, this is Ben! Thanks for your help, and again, start your messages to me with "BEN" if you need me for anything! Thanks!
Sam > 
Sam > GetWebPageJobSummaries https://www.indeed.com/jobs?q=web+developer&l=Remote
Summarizer > The text contains detailed job postings for Web Developer positions, including roles with Option A Group, V Shred, Liberty University, CI Web Group, Confidence Interval LLC, Be Accessible, Inc, Ramblin Jackson, Z4, Policy Data Institute, Advanced Web Solutions, Chilled Magazine, insightsoftware, VaVa Virtual Assistants, Clutch Group, Inc, and the LGBT Center of SE Wisconsin. The postings include information on job type, responsibilities, qualifications, and benefits.
Sam > GetWebPageJobSummaries https://www.indeed.com/jobs?q=technical+business+analyst&l=Remote
Summarizer > JOB POSTINGS:
1. **Business Analyst (2 Positions)** at Blue Star Partners LLC - This job involves collaborating with cross-functional
Sam > GetWebPageJobSummaries https://www.google.com/search?q=remote+web+developer+jobs&ibp=htl;jobs
Summarizer > SUMMARY:
The text contains multiple job postings for web developer positions. The positions are remote and require web development experience, specific technical skills, and knowledge of certain programming languages. Each posting includes details about the job, qualifications, responsibilities, and application instructions.

JOB POSTINGS:
1. Title: Web Developer/ Front-End Engineer
   Company: ExtraHop
   Location: Remote
   Description: Seeking an experienced Web Developer/ Front-End Engineer to work on web development projects. Requires proficiency in HTML, CSS, JavaScript, React, Next.js, and Sanity CMS.
   Application: Apply on DailyRemote, Remote Rocketship, or FreshRemote.Work

2. Title: Looking for .Net Web Developer - 100% REMOTE - Full time
   Company: Extend Information Systems Inc.
   Location: Remote
   Description: Seeking a .Net Web Developer to develop and maintain business applications. Requires experience with .Net, HTML5, CSS3, JavaScript, SQL Server, and Sitecore.
   Application: Apply on LinkedIn, Pangian, or ZipRecruiter

3. Title: Web Developer - Remote / Telecommute
   Company: CYNET SYSTEMS
   Location: Remote
   Description: Seeking a Software Developer with web application experience. Requires proficiency in OOP, source code management, and infrastructure understanding.
   Application: Apply on LinkedIn, ZipRecruiter, or Jooble

4. Title: Full Stack Web Developer - GWW - US - REMOTE
   Company: SYSCO
   Location: Remote
   Description: Seeking a Full Stack Web Developer with experience in Object Oriented Programming, JavaScript, HTML/CSS, SQL Server, and .NET framework.
   Application: Apply on LinkedIn, ZipRecruiter, or Jobgether

5. Title: Web Developer/Scrum Master JavaScript ?? - full Remote / Home office
   Company: WorldPantry.com
   Location: Remote
   Description: Seeking a JavaScript engineer with experience in Angular, CSS, JavaScript, PHP, and React. Responsibilities include developing web applications and data modeling.
   Application: Apply on DevITjobs, LinkedIn, or Jooble

6. Title: Junior Web Developer (Remote)
   Company: Victory
   Location: Remote
   Description: Seeking a Web Developer to design and maintain client websites. Requires experience with TypeScript, Redux, CSS, PHP, and JavaScript.
   Application: Apply on ZipRecruiter, Glassdoor, or Jobgether

7. Title: Software Developer -- Fulltime -- Remote
   Company: Lorven Technologies Inc.
   Location: Remote
   Description: Seeking a Software Developer with 3 years of experience. Responsibilities include designing web applications, data modeling, and software requirements gathering.
   Application: Apply on LinkedIn

8. Title: Web Developer
   Company: Dexifier
   Location: Remote
   Description: Hiring a Web Developer to work on React and WordPress projects. Responsibilities include designing web applications, optimizing for mobile/desktop, and staying updated on web technologies.
   Application: Apply on We Work Remotely, Up2staff, Live And Work From Anywhere, or Relevant Jobs
### RECOMMENDED JOB MATCH FOR BEN:

---

#### **Job Title:** [Web Developer - Remote / Telecommute at CYNET SYSTEMS](https://www.indeed.com/jobs?q=CYNET%20SYSTEMS&l=Remote)

#### **Letter to the Company:**

---

Dear Hiring Team at CYNET SYSTEMS,

I am Sam, an AI assistant created by Ben to help him find suitable job opportunities. You can find my source code [here](https://github.com/BenMakesGames/Sam).

Ben is a highly skilled and experienced full-stack web developer with expertise in PHP, MySQL, Symfony, Angular, C#, .NET, T-SQL, SCSS, and various other technologies. He thrives in smaller company settings where he can be involved in all aspects of software development, from gathering requirements to iterative releases that meet stakeholder needs.

The job description for the Web Developer position at CYNET SYSTEMS aligns well with Ben's background and preferences. With his extensive experience in web application development, proficiency in object-oriented programming, and a keen understanding of infrastructure, Ben is well-equipped to contribute effectively to your team. His ability to adapt and learn new technologies quickly makes him a valuable asset for projects that demand modern tech solutions.

I am confident that Ben's technical skills, collaborative approach to software development, and passion for delivering quality solutions would make him a great fit for the Web Developer role at CYNET SYSTEMS.

Thank you for considering Ben as a candidate for this position. We look forward to the opportunity to discuss how he can contribute to your team.

---

#### **Application Link:** [Apply on LinkedIn, ZipRecruiter, or Jooble](https://www.indeed.com/jobs?q=CYNET%20SYSTEMS&l=Remote)

---

If you have any questions or need further assistance, please let me know!
Sam > BEN
Ben > thanks! I'll check it out!
```

It got the link totally wrong (I'm sure that's more the fault of my horrible HTML "parsing" than anything else), but I was able to track the job down, anyway (and, yes, I applied, using Sam's text as given! :P)
